import { Input} from "@/Components/ui/input";
import { Button } from "@/Components/ui/button";
import { VITE_API_URL } from "../config";
import {useEffect, useRef, useState} from 'react';
import backgroundAuth from './assets/background-auth.svg';
import { useNavigate } from 'react-router-dom';
import { api } from "../api/client";
import { toast } from "sonner";
const AuthPanel = () => {
    const [mode, setMode] = useState("register");
    const formRef = useRef();
    const navigate = useNavigate();
    //i should make this page responsible later
    const handleCredentialsResponse = async (googleResponse) => {
        const toastId = toast.loading("Signing in...");
        const {data, error} = await api.post(`/api/user/auth-google?googleClientToken=${googleResponse.credential}`,
            {}
        );
        if(error){
            toast.dismiss(toastId);
            return;
        }
        localStorage.setItem("token", data);
        toast.success("Logged in", {id: toastId});
        navigate("/");
    };
    
    const handleRegistration = async (e) => {
        e.preventDefault();
        const toastId = toast.loading("Creating account...")
        const {data, error} = await api.post("/api/user/register", {
            email: formRef.current.email.value,
            plainPassword: formRef.current.password.value,
            firstName: formRef.current.first_name.value,
            lastName: formRef.current.last_name.value,
            location: formRef.current.location.value
        })
        if(error)
        {   
            toast.dismiss(toastId)
            return; 
        }
        toast.success("Account created. Please log in", {id: toastId});
        setMode("login");
    }

    const handleLogin = async (e) => {
        e.preventDefault();
        const toastId = toast.loading("Login...");
        const {data, error} = await api.post("/api/user/login", {
            Email: formRef.current.email.value,
            Password: formRef.current.password.value
        })
        if(error){
            toast.dismiss(toastId);
            return;
        }
        localStorage.setItem("token", data);
        navigate('/');
    }

    useEffect(() => {
        if(!window.google){
            console.error("Google script was not loaded")
            return;
        }

        window.google.accounts.id.initialize({
            client_id: "684390088485-7m1i4sbd91l80momqf8ekjcf78s0rvm2.apps.googleusercontent.com",
            callback: handleCredentialsResponse,
            use_fedcm_for_prompt: true
        });

        window.google.accounts.id.renderButton(
            document.getElementById("google-button"),
            { theme: "outline", size: "large", width: "100px" }
        );
    });

    return (
        <div className="flex items-center justify-between min-h-screen w-full pl-20">
            <div className="flex flex-col justify-center gap-3 bg-white border rounded-lg shadow-sm p-6 w-1/3 h-130">
                {mode === "register" ? ( <>

                <h1 className="text-center font-bold text-2xl text-zinc-600">Sign up</h1>

                <div id="google-button" className="flex justify-center"></div>

                <form ref={formRef} onSubmit={handleRegistration}>

                <div className="first-name-input flex flex-col gap-1">
                    <label className="text-base font-medium text-zinc-600 ml-0.5">First name</label>
                    <Input name="first_name" type="text" placeholder="name"/>
                </div>

                <div>
                    <label className="text-base font-medium text-zinc-600 ml-0.5">Last name</label>
                    <Input name="last_name" type="text" placeholder="surname"/>
                </div>

                <div>
                    <label className="text-base font-medium text-zinc-600 ml-0.5">Location</label>
                    <Input name="location" type="text" placeholder="city"/>
                </div>

                <div className="email-input flex flex-col gap-1">
                <label className="text-base font-medium text-zinc-600 ml-0.5">Email</label>
                <Input name="email" type="email" placeholder="name@example.com"/>
                </div>

                <div>
                    <label className="text-base font-medium text-zinc-600 ml-0.5">Password</label>   
                    <Input name="password" type="password" placeholder="password"/>
                </div>

                <Button type="submit" className="w-full mt-4">Create account</Button>

                </form>
                </>
             ) : (<div>
                 <form ref={formRef} onSubmit={handleLogin}>
                <h1 className="text-center font-bold text-2xl">Login</h1>

                <div id="google-button" className="flex justify-center my-3 "></div>

                <div className="email-input flex flex-col gap-1">
                    <label className="text-base font-medium text-zinc-600 ml-0.5">Email</label>
                    <Input name="email" type="email" placeholder="name@example.com"/>
                </div>

                <div>
                    <label className="text-base font-medium text-zinc-600 ml-0.5">Password </label>
                    <Input name="password" type="password" placeholder="password"/>
                </div>

                <Button type="submit" className="w-full mt-4">Login</Button>
                </form>
             </div>)}
             <Button 
                type="button" 
                variant="text"
                className="w-full mt-2 text-zinc-500 hover:text-zinc-800 "
                onClick={() => setMode(mode === "register" ? "login" : "register")}
                >
                {mode === "register" ? "Already have an account? Sign In" : "Don't have an account? Sign Up"}
                </Button>
            </div>
            <div className="w-1/3 h-screen">
                <img src={backgroundAuth} alt="background-image" className="w-full h-full object-cover" />
            </div>
        </div>
    );
}
export default AuthPanel;