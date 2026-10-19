import {toast} from "sonner";
import { VITE_API_URL } from "@/config";

export async function apiRequest(url, options = {}){
    const fullUrl = `${VITE_API_URL}${url}`;

    const isFormData = options.body instanceof FormData;
    const headers = {
        "Authorization": `Bearer ${localStorage.getItem("token")}`,
        ...(isFormData ? {} : { "Content-Type": "application/json" }),
        ...(options.headers || {}),
    };

    try{
        const response = await fetch(fullUrl, {...options, headers});

        if(!response.ok){
            let message = "Something went wrong";

            try{
                const body = await response.json();
                message = body.errors?.[0] || body.message || message;
            }
            catch{

            }
            if(response.status >= 500){
                message = "Server error. Try again later";
            }

            toast.error(message);
            return {data: null, error: message};
        }

        if (response.status === 204) {
            return { data: null, error: null };
        }

        const contentType = response.headers.get("content-type");
        if(contentType && contentType.includes("application/json")){
            const data = await response.json().catch(() => null);
            return {data, error: null};
        }
        else{
            const data = await response.text();
            return {data, error: null};
        }
    }
    catch(ex){
        const message = "Network error. Check the connection"
        toast.error(message);
        console.error(ex);
        return {data: null, error: message};
    }
}

export const api = {
    get: (url) => apiRequest(url, {method: "GET"}),

    post: (url, body) => apiRequest(url, {
        method: "POST",
        body: body instanceof FormData ? body : JSON.stringify(body)
    }),

    put: (url, body) => apiRequest(url, {
        method: "PUT",
        body: body instanceof FormData ? body : JSON.stringify(body)
    }),

        delete: (url, body) => apiRequest(url, { 
        method: "DELETE", 
        body: body ? JSON.stringify(body) : undefined 
    })
}