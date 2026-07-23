import { VITE_API_URL } from "../config";
import {useEffect, useState} from 'react';
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/Components/ui/table"
import {Tooltip, TooltipContent, TooltipTrigger } from "@/Components/ui/tooltip"
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious, } from "@/Components/ui/pagination" 
import { Checkbox } from "@/Components/ui/checkbox"
import createButtonIcon from "./assets/create-button.svg";
import editButtonIcon from "./assets/edit-button.svg";
import { Input} from "@/Components/ui/input";
import { Loader2 } from "lucide-react"
import { Button } from "@/Components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue, } from "@/Components/ui/select"
import deleteButtonIcon from "./assets/delete-button.svg";
import { Dialog, DialogTrigger, DialogContent, DialogTitle, DialogHeader} from "@/Components/ui/dialog"
const AttributePanel = () => {
    const [attributes, setAttributes] = useState([]);
    const [attributePrefix, setAttributePrefix] = useState("");
    const [categories, setCategories] = useState([]);
    const [category, setCategory] = useState("");
    const [name, setName] = useState("");
    const [totalPages, setTotalPages] = useState(1);
    const [currentPage, setCurrentPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const [selectedIds, setSelectedIds] = useState([]);
    const [dataType, setDataType] = useState("string");
    const [editingId, setEditingId] = useState("");
    const [editingMode, setEditingMode] = useState(false);
    const [filterCategory, setFilterCategory] = useState("");
    const [options, setOptions] = useState([]);
    const [isModalOpen, setModalOpen] = useState(false);
    const [optionInputValue, setOptionInputValue] = useState("");
    const itemsPerPage = 11;//tooltip for toolbar I should make it later
    const [version, setVersion] = useState(0);
    useEffect(() => {
        getAttributesData();
    }, [currentPage, attributePrefix, filterCategory]);

    useEffect(() => {
        getCategories();
    }, []);

    const handleOpenChange = (open) => {
        if(open && selectedIds.length > 1){
            return;
        }
        if (!open) {
            resetForm(); 
        }
        setModalOpen(open);
    }

    const toggleSelectedRow = (id) => {
        setSelectedIds(prev => prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]);
    }

    const handleAdd = () => {
        if(optionInputValue.trim() != ""){
            setOptions([...options, optionInputValue.trim()]);
            setOptionInputValue("");
        }
    }

    const handleOpenEdit = () => {
        if (selectedIds.length !== 1) return;

        const targetId = selectedIds[0];
        const attribute = attributes.find(attr => attr.id === targetId);
        console.log(attribute);
        if (attribute) {
            setEditingMode(true);

            setEditingId(attribute.id); 
            setName(attribute.name);
            setVersion(attribute.version);
            setCategory(attribute.categoryName); 
            setDataType(attribute.dataType.charAt(0).toLowerCase() + attribute.dataType.slice(1));
            setOptionInputValue("");
            setOptions(attribute.attributeOptions || []);
            setModalOpen(true); 
        }
    };

    const resetForm = () => {
        setEditingMode(false);
        setEditingId("");
        setName("");
        setCategory("");
        setDataType("");
        setOptions([]);
        setOptionInputValue("");
        setVersion(0);
    };

    const handleRemove = (index) => {
        setOptions(options.filter((_, i) => i != index));
    }

    const createAttribute =  async() => {
        try{
            setIsLoading(true);
            const response = await fetch(`${VITE_API_URL}/api/attribute/attribute`, {
                method: 'POST',
                headers: {
                    'Authorization': "bearer " + localStorage.getItem("token"),
                    'Content-Type': "application/json"
                },
                body: JSON.stringify({
                    Name: name,
                    DataType: dataType,
                    AttributeCategory: category,
                    AttributeOptions: options
                })
            })
            if(!response.ok){
                console.error(response.error);
            }
            else{
                getAttributesData();
                resetForm();
                setModalOpen(false);
            }
        }
        catch(ex){
            console.error(ex);
        }
        finally{
            setIsLoading(false);   
        }
    }

    const getCategories = async () => {
        try{
            const response = await fetch(`${VITE_API_URL}/api/attribute/categories`, {
                method: 'GET',
                headers: {
                    'Authorization': "bearer " + localStorage.getItem("token"),
                    'Content-Type': "application/json"
                }});
            
                if(!response.ok){
                    console.error(response.error);
                    return;
                }
                else{
                    const data = await response.json();
                    console.log(data);
                    setCategories(data);
                }
        }
        catch(ex){
            console.error(ex);
        }
    }


    const editAttribute = async () => {
       try{
            setIsLoading(true);
           const response = await fetch(`${VITE_API_URL}/api/attribute/attribute`, {
                method: 'PUT',
                headers: {
                    'Authorization': "bearer " + localStorage.getItem("token"),
                    'Content-Type': "application/json"
                },
                body: JSON.stringify({
                    Id: editingId,
                    Name: name,
                    DataType: dataType,
                    Version: version,
                    AttributeCategory: category,
                    AttributeOptions: options
                })
           });
           if(!response.ok){
                console.error(response.error);
           }
           else{
                getAttributesData();
                resetForm();
                setModalOpen(false);
           }
       }
       catch(ex){
        console.error(ex);
       }
       finally{
            setIsLoading(false);   
        }
    }

    const deleteAttributes = async () => {
        try{
            const response = await fetch(`${VITE_API_URL}/api/attribute/attributes`, {
                method: 'DELETE',
                headers: {
                    'Authorization': "bearer " + localStorage.getItem("token"),
                    'Content-Type': "application/json"
                },
                body: JSON.stringify(selectedIds)
            });
            if(!response.ok){
                console.error(await response.text());
                return;
            }
            else{
                getAttributesData();
                setSelectedIds([]);
            }
        }
        catch(ex){
            console.error(ex);
        }
    }


    const getAttributesData = async () => {
        try{
            const params = new URLSearchParams();
            if(attributePrefix.trim() != ""){
                params.append("prefix", attributePrefix.trim());
            }
            if(filterCategory && filterCategory.trim() != ""){
                params.append("category", filterCategory.trim());
            }
            params.append("page", currentPage);
            params.append("pageSize", itemsPerPage);

            const response = await fetch(`${VITE_API_URL}/api/attribute/attributes?${params.toString()}`, {
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
                setAttributes(data.attributeResponseDTO);
                setTotalPages(data.totalItems);
                }   
            }
        catch(ex){
            console.error(ex);
        }
    }
    
    return (
        <div className="flex flex-col justify-start pt-17 items-center min-h-screen">
            <div className="w-full max-w-6xl px-12 py-5 h-142 shadow-[0_0_25px_rgba(0,0,0,0.1)]">
                <div className="border-b h-10 items-center flex gap-7">
                    <Dialog open={isModalOpen} onOpenChange={handleOpenChange}>
                        <DialogTrigger asChild>
                            <div className="flex items-center gap-7">
                                 <Tooltip>
                                    <TooltipTrigger asChild>
                                        <img src={createButtonIcon} alt="create-button" className="w-6.5 h-6.5 cursor-pointer" />
                                    </TooltipTrigger>
                                    <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                        <span>Create new attribute</span>
                                    </TooltipContent>
                                </Tooltip>
                            </div>
                        </DialogTrigger>
                        <Tooltip>
                        <TooltipTrigger asChild>
                        <img onClick={handleOpenEdit} src={editButtonIcon} alt="edit-button" 
                         className={`h-7 w-7 ${selectedIds.length > 1 ? "opacity-45 pointer-events-none" : "cursor-pointer"}`} />
                        </TooltipTrigger>
                        <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                            <span>Edit attribute</span>
                        </TooltipContent>
                     </Tooltip>
                        <DialogContent onOpenAutoFocus={(e) => e.preventDefault()} onPointerDownOutside={(e) => e.preventDefault()} className="h-120 w-130 sm:max-w-250 flex flex-col">
                            <DialogHeader className="flex items-center pt-2 mb-5">
                                <DialogTitle className="font-medium text-base font-sans">{editingMode ? "Edit attribute" : "Create new attribute"}</DialogTitle>
                            </DialogHeader>

                            <div className="flex flex-col gap-1 justify-start">
                                <label className="text-base font-medium text-zinc-600 ml-0.5">Attribute name</label>
                                <Input name="name" type="text" value={name} onChange={(e) => setName(e.target.value)} className="max-w-70"placeholder="name"/>
                            </div>

                            <Select value={dataType} onValueChange={setDataType}>
                                <SelectTrigger className="w-70">
                                    <SelectValue placeholder="Data Type" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="string">String</SelectItem>
                                    <SelectItem value="text">Text</SelectItem>
                                    <SelectItem value="image">Image</SelectItem>
                                    <SelectItem value="numeric">Numeric</SelectItem>
                                    <SelectItem value="date">Date</SelectItem>
                                    <SelectItem value="period">Period</SelectItem>
                                    <SelectItem value="boolean">Boolean</SelectItem>
                                    <SelectItem value="oneOfMany">Select</SelectItem>
                                </SelectContent>
                            </Select>

                            <Select value={category} onValueChange={setCategory}>
                                <SelectTrigger className="w-70">
                                    <SelectValue placeholder="Attribute category" />
                                </SelectTrigger>
                                <SelectContent>
                                    {categories.map((category, index) => (
                                        <SelectItem key={index} value={category}>{category}</SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            { dataType === "oneOfMany" &&
                            <div className="flex flex-col gap-2">
                                <label className="text-base font-medium text-zinc-600">Options</label>
                                <div className="flex gap-2">
                                    <Input value={optionInputValue} onChange={(input) => setOptionInputValue(input.target.value)}
                                     placeholder="Add option..." className="w-70"/>
                                    <Button type="button" onClick={handleAdd}>Add</Button>
                                </div>
                                <div className="flex flex-wrap gap-1.5 mt-2">
                                    {options.map((option, index) => (
                                        <div key={index} className="flex items-start gap-1.5 border rounded-md text-sm px-2.5 py-1">
                                            <span>{option}</span>
                                            <button type="button" onClick={() => handleRemove(index)} className="text-zinc-400 hover:text-zinc-800 transition-colors">
                                                 ×
                                            </button>
                                        </div>   
                                    ))}
                                </div>
                            </div>
                            }
                            <Button  disabled={isLoading}  className="w-28 absolute bottom-0 mb-4"onClick={editingMode ? editAttribute : createAttribute}>
                                {isLoading ? (<><Loader2 className="mr-2 h-4 w-4 animate-spin"/> Saving... </>) : "Save"}
                            </Button>
                        </DialogContent>
                    </Dialog>
                    <Tooltip>
                        <TooltipTrigger asChild>
                            <img onClick={deleteAttributes} src={deleteButtonIcon} alt="delete-button" className="w-8 h-8 cursor-pointer"/>
                        </TooltipTrigger>
                        <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                            <span>Delete new attribute</span>
                        </TooltipContent>
                     </Tooltip>

                     <Select value={filterCategory} onValueChange={setFilterCategory}>
                        <SelectTrigger>
                            <SelectValue placeholder="Attribute category" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="">All</SelectItem>
                             {categories.map((category, index) => (
                                <SelectItem key={index} value={category}>{category}</SelectItem>
                            ))}
                        </SelectContent>
                     </Select>

                     <div className="flex flex-col gap-2">
                        <Input id="attributeSearch"
                        type="text" placeholder="Search attribute..." value={attributePrefix} onChange={(e) => setAttributePrefix(e.target.value)}/>
                    </div>


                </div>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead className="w-12.5">
                            </TableHead>
                            <TableHead >Name</TableHead>
                            <TableHead>Data Type</TableHead>
                            <TableHead>Category</TableHead>
                            <TableHead>Options</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {attributes.map((attribute) => {
                            const isSelected = selectedIds.includes(attribute.id);
                            return (<TableRow key={attribute.id}
                                onClick={() => toggleSelectedRow(attribute.id)}
                                className="cursor-pointer transition-colors"
                                data-state={isSelected ? "selected" : ""}>
                                <TableCell className="w-12.5">
                                    <Checkbox 
                                        checked={isSelected}
                                    />
                                </TableCell>
                                <TableCell>{attribute.name}</TableCell>
                                <TableCell>{attribute.dataType}</TableCell>
                                <TableCell>{attribute.categoryName}</TableCell>
                                <TableCell className="text-right cursor-pointer hover:text-zinc-600">
                                    {attribute.attributeOptions.length > 0 ?
                                    (
                                            <Tooltip>
                                                <TooltipTrigger asChild>
                                                    <span className="hover:text-zinc-700">{attribute.attributeOptions.length} options</span>
                                                </TooltipTrigger>
                                                <TooltipContent className="font-medium font-sans bg-white text-black border-2">    
                                                    <ul>
                                                        {attribute.attributeOptions.map((option, i) => (
                                                            <li key={i}>• {option}</li>
                                                        ))}
                                                    </ul>
                                                </TooltipContent>
                                            </Tooltip>
                                    ) : "—"}
                                </TableCell>

                            </TableRow>)
                            })}
                    </TableBody>
                </Table>
                <Pagination className="border-t pt-5">
                    <PaginationContent>
                        <PaginationItem>
                            <PaginationPrevious className="cursor-pointer transition-colors hover:bg-zinc-100!" onClick={() => setCurrentPage(prev => prev == 1 ? 1 : prev - 1)}/>
                        </PaginationItem>
                        <PaginationItem>
                            <PaginationLink>{currentPage}</PaginationLink>
                        </PaginationItem>
                        <PaginationItem>
                            <PaginationNext 
                                className="cursor-pointer hover:bg-zinc-100!" 
                                onClick={() => setCurrentPage(prev => prev >= totalPages ? totalPages : prev + 1)} 
                            />
                        </PaginationItem>
                    </PaginationContent>
                </Pagination>
                </div>
        </div>
    );
}
export default AttributePanel;