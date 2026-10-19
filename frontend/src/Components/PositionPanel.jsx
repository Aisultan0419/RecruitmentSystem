import { VITE_API_URL } from "../config";
import {useEffect, useState, useRef} from 'react';
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/Components/ui/table"
import {Tooltip, TooltipContent, TooltipTrigger } from "@/Components/ui/tooltip"
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious, } from "@/Components/ui/pagination" 
import createButtonIcon from "./assets/create-button.svg";
import editButtonIcon from "./assets/edit-button.svg";
import { Input } from "@/Components/ui/input";
import { Label } from "@/Components/ui/label";
import { Button } from "@/Components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue, } from "@/Components/ui/select"
import deleteButtonIcon from "./assets/delete-button.svg";
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList, } from "@/Components/ui/combobox"
import { Dialog, DialogTrigger, DialogContent, DialogTitle, DialogHeader} from "@/Components/ui/dialog"
import { ShieldCheck, Shield } from "lucide-react";
import { api } from "../api/client";
import { toast } from "sonner";
const PositionPanel = () => {

    const [selectedIds, setSelectedIds] = useState([]);
    const [isModalOpen, setModalOpen] = useState(false);
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [maxProjectsCount, setMaxProjectsCount] = useState(0);
    const [attributes, setAttributes] = useState([]);
    const [attribute, setAttribute] = useState();
    const dialogContentRef = useRef(null);
    const [attributePrefix, setAttributePrefix] = useState("");
    const [filterCategory, setFilterCategory] = useState("");
    const [categories, setCategories] = useState([]);
    const [version, setVersion] = useState(0);
    const [positionId, setPositionId] = useState(null);
    const [userValue, setUserValue] = useState("");
    const [pickedAttributes, setPickedAttributes] = useState([]);
    const [saving, setSaving] = useState("");
    const [currentPage, setCurrentPage] = useState(1);
    const itemPerPage = 6;
    const toggleSelectedRow = (id) => {
        setSelectedIds(prev => prev.includes(id) ? prev.filter(prev => prev.id !== id) : [...prev, id])
    }
    useEffect(() => {
        getCategories();
    }, []);

    useEffect(() => {
            getAttributesData();
        }, [attributePrefix, filterCategory, isModalOpen]);

    useEffect(() => {
        if(saving !== "Saved") return;
        const timer = setTimeout(() => setSaving(""), 3000);
        return () => clearTimeout(timer);
    }, [saving]);

    useEffect(() => {
        if(!title.trim() || !description.trim()){
            return;
        }

        const timer = setTimeout(async () => {
            if(positionId === null){
                createPosition();
            }
            else{
                updatePosition();
            }
        }, 1000);

        return () => {
            clearTimeout(timer);
        };

    }, [title, description, maxProjectsCount])

    const getAttributesData = async () => {
        const params = new URLSearchParams();
        if(attributePrefix.trim() != ""){
            params.append("prefix", attributePrefix.trim());
        }
        if(filterCategory && filterCategory.trim() != ""){
            params.append("category", filterCategory.trim());
        }
        params.append("page", 1);
        params.append("pageSize", itemPerPage);
        const {data, error} = await api.get(`/api/attribute/attributes?${params.toString()}`);
        if(error){
            return;
        }
        setAttributes(data.attributeResponseDTO);
    }
    const getCategories = async () => {
        const {data, error} = await api.get(`/api/attribute/categories`);
        if(error){
            return;
        }
        setCategories(data);
    }

    const updatePosition = async () => {
        const toastId = toast.loading("Saving...") 
        const {data, error} = await api.put("/api/position/update", {
            Id: positionId,
            Title: title,
            Description: description,
            MaxProjectsCount: maxProjectsCount,
            Version: version
        });
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        setVersion(data);
    }

    const addAttribute = async(attribute, positionId) => {
        const toastId = toast.loading("Saving...");
        const {error} = await api.post(`/api/position/attribute?AttributeId=${attribute.id}&PositionId=${positionId}`, {});
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        setPickedAttributes(prev => [...prev, attribute]);
    }

    const deleteAttribute = async (attribute, positionId) => {
        const toastId = toast.loading("Deleting attribute...")
        const {error} = await api.delete(`/api/position/attribute?AttributeId=${attribute.id}&PositionId=${positionId}`);
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Attribute is deleted", {id: toastId});
        setPickedAttributes(prev => prev.filter(item => item.id !== attribute.id));
    }

    const createPosition = async () => {
        const toastId = toast.loading("Saving data...");
        const {data, error} = await api.post("/api/position/create", {
            Title: title,
            Description: description,
            maxProjectsCount: maxProjectsCount
        })
        if(error) {
            toast.dismiss(toastId);
            return;
        }
        toast.success("Data saved", {id: toastId})
        setPositionId(data);
    }

    const resetForm = () => {
        setUserValue("");
        setAttributePrefix("");
        setFilterCategory("");
        setAttribute(null);
        setVersion(0);
        setPositionId(null);
        setTitle("");
        setDescription("");
        setMaxProjectsCount(0);
    }

    const handleOpenChange = (open) => {
        if(open && selectedIds.length > 1){
            return;
        }
        if (!open) {
            resetForm(); 
        }
        setModalOpen(open);
    }

    return (
        <>
            <div className="flex flex-col justify-start items-center min-h-screen pt-17">
                <div className="w-full rounded-xl max-w-6xl px-12 py-5 h-142 shadow-[0_0_25px_rgba(0,0,0,0.1)]">
                    <div className="border-b h-10 items-center flex gap-7">
                        <Dialog open={isModalOpen} onOpenChange={handleOpenChange}>
                            <DialogTrigger asChild>
                                <div className="flex gap-7 items-center">
                                    <Tooltip>
                                        <TooltipTrigger asChild>
                                            <img src={createButtonIcon} alt="create-button" className="w-6.5 h-6.5 cursor-pointer" />
                                        </TooltipTrigger>
                                        <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                            <span>Create new position</span>
                                        </TooltipContent>
                                    </Tooltip>
                                </div>
                            </DialogTrigger>
                            <Tooltip> 
                                <TooltipTrigger asChild>
                                    <img className={`h-7 w-7 ${selectedIds.length > 1 ? "opacity-45 pointer-events-none" : "cursor-pointer"}`} src={editButtonIcon} alt="edit-button" />
                                </TooltipTrigger>
                                <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                    <span>Edit attribute</span>
                                </TooltipContent>
                            </Tooltip>
                            <DialogContent relative ref={dialogContentRef} onOpenAutoFocus={e => e.preventDefault()} onPointerDownOutside={e => e.preventDefault()} className="sm:max-w-250 h-150 w-220 flex flex-col">
                                <DialogTitle className="flex justify-center mt-2">Create new position</DialogTitle>
                                <div className="flex flex-row">
                                    <div className="w-1/2 flex-none"> {/* Left column: form */}
                                        <div className="flex flex-col justify-start  m-4">
                                            <label className="text-small text-zinc-600 font-medium">Title</label>
                                            <Input type="text" value={title} onChange={e => setTitle(e.target.value)} className="max-w-100"></Input>
                                            <label className="text-small text-zinc-600 font-medium">Description</label>
                                            <Input type="text" value={description} onChange={e => setDescription(e.target.value)} className="max-w-100"></Input>
                                            <label className="text-small text-zinc-600 font-medium">Maximum projects number</label>
                                            <Input type="number" value={maxProjectsCount} onChange={e => setMaxProjectsCount(e.target.value)} min={0} className="max-w-100"></Input>
                                        </div>
                                        <div className="flex flex-col ml-4 h-full  gap-7">
                                            <div className="flex flex-row gap-5.5">
                                                <Combobox 
                                                    items={attributes}
                                                    value={attribute}
                                                    className="flex-none"
                                                    onValueChange={(val) => {
                                                        setAttribute(val);
                                                        if (val) {
                                                            setAttributePrefix(val.name); 
                                                        }
                                                    }}
                                                    itemToStringLabel={(item) => item?.name ?? ""}
                                                    filter={null}
                                                    >
                                                    <ComboboxInput 
                                                        value={attributePrefix} 
                                                        onChange={(e) => setAttributePrefix(e.target.value)}  
                                                        placeholder="Select an attribute" 
                                                    />
                                                    <ComboboxContent container={dialogContentRef.current}>
                                                        <ComboboxEmpty>No attributes found.</ComboboxEmpty>
                                                        <ComboboxList>
                                                        {(item) => (
                                                            <ComboboxItem key={item.id} value={item}>
                                                            {item.name}
                                                            </ComboboxItem>
                                                        )}
                                                        </ComboboxList>
                                                    </ComboboxContent>
                                                </Combobox>

                                                <Select className="flex-none" value={filterCategory} onValueChange={setFilterCategory}>
                                                    <SelectTrigger>
                                                        <SelectValue placeholder="Attribute category" />
                                                    </SelectTrigger>
                                                    <SelectContent className="w-18">
                                                        <SelectItem value="">All</SelectItem>
                                                        {categories.map((category, index) => (
                                                            <SelectItem key={index} value={category}>{category}</SelectItem>
                                                        ))}
                                                    </SelectContent>
                                                </Select>
                                            </div>
                                            <div className="flex flex-col gap-5">

                                                {attribute && (
                                                    <div className="ml-2 flex flex-col gap-2"> 
                                                        <div>
                                                            <Label className="text-zinc-600 text-[13px]">Data type</Label>
                                                            <span className="text-[15px] font-sans">{attribute.dataType}</span>
                                                        </div>
                                                        <div>
                                                            <Label className="text-zinc-600 text-[13px]">Attribute category</Label>
                                                            <span className="text-[15px] font-sans">{attribute.categoryName}</span>
                                                        </div>
                                                        {attribute.dataType === "OneOfMany" && (
                                                            <div className="flex flex-wrap gap-2">
                                                                <Label>Attribute options</Label>
                                                                {attribute.attributeOptions.map((option, index) => (
                                                                    <div className="border p-1 rounded-md bg-zinc-100 overflow-y-auto max-h-50 border-zinc-400 ">
                                                                        <span key={index} className="text-[15px] font-sans">{option}</span>
                                                                    </div>
                                                                ))}
                                                            </div>
                                                        )}
                                                    </div>
                                                )}

                                            
                                                <div className="mb-15 max-w-70">
                                                    {attribute && <Button onClick={() => positionId === null ? console.log("position is not created") : addAttribute(attribute, positionId)}>Add</Button>}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="flex flex-col w-1/2 flex-none items-center"> {/* Right column: attributes */}
                                        <span className="text-base">Chosen attributes</span>
                                        <div className="flex flex-row flex-wrap w-full min-w-0 overflow-y-auto max-h-100 mt-2">
                                            {pickedAttributes.length !== 0 ? (
                                                pickedAttributes.map((option, index) => (
                                                    <div className="w-100 h-10 
                                                     flex text-center items-center border-b justify-between px-2
                                                         cursor-pointer truncate hover:bg-zinc-50 transition-colors">
                                                        <span key={index} className="font-medium text-[14px] ">{option.name}</span>
                                                        <button className="cursor-pointer ml-4 text-zinc-500 hover:text-zinc-900" 
                                                        onClick={() => deleteAttribute(option, positionId)}>x</button>
                                                    </div>
                                                ))) : (<span className="text-zinc-600 mt-4"> No attributes chosen yet </span>)
                                            }
                                        </div>
                                    </div>
                                </div>
                                <div className="max-w-100 absolute bottom-0 mb-4 flex items-center mt-auto gap-6">
                                    <Button className="w-25">Publish</Button>

                                    <div className="relative text-base text-zinc-600 font-medium  w-20 h-6">
                                        <span
                                            className={`absolute inset-0  text-blue-400 transition-opacity duration-300 ${
                                                saving === "Saving..." ? "opacity-100" : "opacity-0"
                                            }`}>
                                            Saving...
                                        </span>
                                        <span
                                            className={`absolute inset-0 text-green-600 transition-opacity duration-300 ${
                                                saving === "Saved" ? "opacity-100" : "opacity-0"
                                            }`}>
                                            Saved
                                        </span>
                                    </div>
                                </div>
                            </DialogContent>
                        </Dialog>
                    </div>
                </div>
            </div>
        </>
    );
}
export default PositionPanel;   