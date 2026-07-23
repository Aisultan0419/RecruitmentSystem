import {Card, CardHeader, CardTitle, CardContent, CardFooter} from "@/Components/ui/card"
import {Avatar, AvatarImage, AvatarFallback} from "@/Components/ui/avatar"
import React, {useState, useEffect} from 'react';
import Dropzone from 'react-dropzone'
import { Button } from "@/Components/ui/button"
import { Input } from "@/Components/ui/input"
import { Label } from "@/Components/ui/label"
import { VITE_API_URL } from "../config"; //user errors я еще их не сделал для пользователя, пока что только логирую
import { Dialog, DialogTrigger, DialogContent, DialogTitle, DialogHeader, DialogFooter} from "@/Components/ui/dialog"
import { jwtDecode } from "jwt-decode";
import CandidatePanel from "./CandidateAttributePanel";
const MePanel = () => {

    const [isEditing, setIsEditing] = useState(false); 
    const [firstName, setFirstName] = useState("");
    const [secondName, setSecondName] = useState("");
    const [location, setLocation] = useState("");
    const [avatarUrl, setAvatarUrl] = useState(null);
    const [loading, setLoading] = useState("");
    const [version, setVersion] = useState(0);
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    function isEmptyOrSpaces(str) {
        return str === null || str === undefined || str.trim() === '';
    }

    useEffect(() => {
        getProfile();
    }, []);

    const getProfile = async () => {
        try{
            const response = await fetch(`${VITE_API_URL}/api/user/profile`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + localStorage.getItem("token"),
                    'Content-Type': 'application/json'
                }});
            
                if(!response.ok)
                    console.error(await response.text());
                else{
                    const data = await response.json();
                    console.log(data);
                    setFirstName(data.firstName);
                    setSecondName(data.lastName);
                    setLocation(data.location);
                    setAvatarUrl(data.photoUrl);
                    setVersion(data.version);  
                }
            }
        catch(ex){
            console.error(ex);
        }
    }

    const editProfile = async(e) => {
        e.preventDefault();

        try{
            const response = await fetch(`${VITE_API_URL}/api/user/profile-modify`, {
                method: 'PUT',
                headers: {
                    'Authorization': 'Bearer ' + localStorage.getItem("token"),
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    FirstName: firstName,
                    LastName: secondName,
                    Location: location,
                    Version: version
                })
            });
            if(!response.ok){
                console.error(await response.text());
                return;
            }
            setIsEditing(false);
            getProfile();
        }
        catch(ex){
            console.error(ex);
        }

    }

    function getInitials(str){
        return str.split(" ").map(word => word[0]).join("").toUpperCase().slice(0, 2);
    }

    function defineId(){
        try{
          let token = localStorage.getItem("token");
          const decoded = jwtDecode(token);
    
          const currentTime = Date.now();
            if (decoded.exp && decoded.exp * 1000 < currentTime) {
              return "unauthorized"; 
            }
          return decoded.sub || "unauthorized";
        }
        catch(ex){
          console.error(ex);
          return "unauthorized";
        }
      }
    
    const applyAvatar = async(file) => {
        const formData = new FormData();
        formData.append("file", file);
        setLoading("Uploading...");
        try{
            const response = await fetch(`${VITE_API_URL}/api/user/upload-avatar`, {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + localStorage.getItem("token")
                },
                body: formData
            });
            if(!response.ok){
                console.error(await response.text()); 
                setLoading("We could not apply your file, try again later");  //I have problems with json/text formats confusion, i need to solve it later
                return;
            }
            else{
                const imageUrl = await response.text();
                setAvatarUrl(imageUrl);
                getProfile();
                setLoading("");
                setIsDialogOpen(false);
                console.log("Avatar is updated")
            }
        }
        catch(error){
            console.error(error);
            setLoading("We could not apply your file, try again later");
        }
    }

    return(
        <div className="flex items-center justify-center gap-20 w-full h-screen">
            <Card className="border flex flex-row h-150 w-230 p-10 justify-between shadow-xl">
                <form onSubmit={editProfile} className="flex h-full flex-col">
                    <CardHeader className="flex justify-start items-center gap-5">
                        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                            <DialogTrigger asChild>
                                <Avatar className="w-24 h-24 relative group cursor-pointer">
                                    <AvatarImage src={avatarUrl || undefined} alt="userAvatar" />
                                    <AvatarFallback className="text-4xl font-medium">
                                        {getInitials(`${firstName} ${secondName}`)}
                                    </AvatarFallback>
                                    <div className="flex justify-center items-center
                                    text-white text-sm font-medium
                                    absolute inset-0 opacity-0 
                                    group-hover:opacity-100 transition-opacity-200 rounded-full
                                    bg-black/40">Modify</div>
                                </Avatar>
                            </DialogTrigger>
                            <DialogContent>
                                <DialogHeader>
                                    <DialogTitle>Upload avatar</DialogTitle>
                                </DialogHeader>
                                <Dropzone onDrop={(acceptedFiles, fileRejections) => {
                                    if(fileRejections.length > 0) {
                                        console.log("You can insert only one file");
                                        return;
                                    }
                                    if(acceptedFiles.length > 0){
                                        const file = acceptedFiles[0];
                                        applyAvatar(file);
                                    }
                                }} maxFiles={1}>
                                    {
                                        ({getRootProps, getInputProps}) => (
                                            <div {...getRootProps()} className="border-2 border-dashed 
                                            border-gray-300 rounded-xl p-8 text-center cursor-pointer 
                                            hover:border-gray-400 transition-colors flex flex-col items-center justify-center
                                            min-h-50">
                                                
                                            <input {...getInputProps()} />

                                            <p className="text-sm text-gray-600 font-medium">Drag your files here or click</p>
                                            <span className="text-sm text-gray-600 font-medium">Supported formats JPG, PNG, WEBP</span>
                                            </div>
                                        )
                                    }
                                </Dropzone>
                                <DialogFooter className="flex flex-row border-0 justify-center items-center sm:justify-center"> {loading} </DialogFooter>
                            </DialogContent>
                        </Dialog>
                        { isEditing ? (
                            <div className="flex flex-row gap-5">

                                <div className="flex flex-col gap-2">
                                    <Label htmlFor="firstName" > First Name </Label>
                                    <Input id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)}
                                    type="text"/>
                                </div>

                                <div className="flex flex-col gap-2">
                                    <Label htmlFor="secondName"> Second name </Label>
                                    <Input id="secondName"
                                    type="text"  value={secondName} onChange={(e) => setSecondName(e.target.value)}/>
                                </div>

                            </div>
                        ) : (
                        <CardTitle className="flex text-bold text-[23px] font-medium">{ ( isEmptyOrSpaces(firstName) && isEmptyOrSpaces(secondName)) ? "Name is not set" : `${firstName} ${secondName}`}</CardTitle>)}
                    </CardHeader> 
                {isEditing ? 
                (
                    <CardContent className="flex flex-col mt-4 ml-2 gap-3">

                        <div className="location flex flex-col gap-2 w-90">
                            <Label htmlFor="location">Location</Label>
                            <Input id="location" type="text" value={location} onChange={(e) => setLocation(e.target.value)}/>
                        </div>

                    </CardContent>
                )
                 : 
                (
                    <CardContent className="mt-8 ml-2">

                        <div className="location">
                            <span className="text-sm font-medium text-muted-foreground">Location</span>
                            <p className="text-base text-foreground">{isEmptyOrSpaces(location) ?  "Location is not set" : location}</p>
                        </div>

                    </CardContent>
                )
                }
                    <div className="flex mt-4 ml-4">
                        {isEditing ? (<Button type="submit">Save</Button>) : (<Button type="button" onClick={(e) => { e.preventDefault(); setIsEditing(true)}}>Edit</Button>)}
                    </div>
                </form>
                <div className="max-w-md self-start h-fit mt-32 "> 
                    <CandidatePanel id={defineId()}/>
                </div>
            </Card>
        </div>
    );
}

export default MePanel;