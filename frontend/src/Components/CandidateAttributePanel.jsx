import React, {useRef, useState, useEffect} from 'react';
import Dropzone from 'react-dropzone'
import { Button } from "@/Components/ui/button"
import { Input } from "@/Components/ui/input"
import { Label } from "@/Components/ui/label"
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList, } from "@/Components/ui/combobox"
import { VITE_API_URL } from "../config"; //user errors я еще их не сделал для пользователя, пока что только логирую
import { Dialog, DialogTrigger, DialogContent, DialogTitle, DialogHeader, DialogFooter} from "@/Components/ui/dialog"
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/Components/ui/table"
import {Tooltip, TooltipContent, TooltipTrigger } from "@/Components/ui/tooltip"
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious, } from "@/Components/ui/pagination" 
import { Checkbox } from "@/Components/ui/checkbox"
import createButtonIcon from "./assets/create-button.svg";
import editButtonIcon from "./assets/edit-button.svg";
import { Loader2 } from "lucide-react"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue, } from "@/Components/ui/select"
import deleteButtonIcon from "./assets/delete-button.svg";


const CandidatePanel = (id) => {
    const [attributes, setAttributes] = useState([]);
    const [attributeValues, setAttributeValues] = useState([]);
    const [attribute, setAttribute] = useState();
    const [attributePrefix, setAttributePrefix] = useState("");
    const [attributeValueDictionary, setAttributeValueDictionary] = useState({});
    const [isModalOpen, setModalOpen] = useState(false);
    const [editingMode, setEditingMode] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [selectedNames, setSelectedNames] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const dialogContentRef = useRef(null);
    const [filterCategory, setFilterCategory] = useState("");
    const [categories, setCategories] = useState([]);
    const itemPerPage = 6;
    const startIndex = (currentPage - 1) * itemPerPage;
    const endIndex = startIndex + itemPerPage;
    const totalPages = Math.ceil(Object.keys(attributeValueDictionary).length / itemPerPage); 
    const paginatedAttributesValues = Object.entries(attributeValueDictionary).slice(startIndex, endIndex);
    useEffect(() => {
        getAttributeValues();
        getCategories();
    }, []);

    useEffect(() => {
        getAttributesData();
    }, [attributePrefix, filterCategory]);

    useEffect(() => {
        if(attributes.length > 0 && attributeValues.length > 0){
            createAttributeValueDictionary();
        }
    }, [attributes, attributeValues]);

    function createAttributeValueDictionary(){
        const dictionary = {};
        const attributeMap = new Map(attributes.map(a => [a.id, a.name]));
        for (const val of attributeValues) {
            const attributeName = attributeMap.get(val.attributeId);
            if (attributeName) {
                dictionary[attributeName] = val.value;
            }
        }
        console.log(dictionary);
        setAttributeValueDictionary(dictionary);
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
            params.append("page", 1);
            params.append("pageSize", itemPerPage);

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
                setAttributes(data);
                }   
            }
        catch(ex){
            console.error(ex);
        }
    }


    const getAttributeValues = async () =>  {
        try{
            console.log(id);
            const response = await fetch(`${VITE_API_URL}/api/attribute/candidate-attributes?userId=${id.id}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }});
            
            if(!response.ok)
                console.error(await response.text());
            else{
                const data = await response.json();
                console.log(data);
                setAttributeValues(data);
                }   
            }
        catch(ex){
            console.error(ex);
        }
    }
    const handleOpenEdit = () => {
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

    const toggleSelectedRow = (name) => {
        setSelectedNames(prev => prev.includes(name) ? prev.filter(item => item !== name) : [...prev, name]);
    }

    const createAttribute = () => {}
    const deleteAttributes = () => {}
    const editAttribute = () => {}
    const resetForm = () => {}

    const handleOpenChange = (open) => {
        if(open && selectedNames.length > 1){
            return;
        }
        if (!open) {
            resetForm(); 
        }
        setModalOpen(open);
    }

    return (<>
        <div className="flex flex-row border-b gap-5">
                <Dialog open={isModalOpen} onOpenChange={handleOpenChange}>
                    <DialogTrigger asChild>
                        <div className="flex items-center gap-7">
                                <Tooltip>
                                <TooltipTrigger asChild>
                                    <img src={createButtonIcon} alt="create-button" className="w-5.5 h-5.5 cursor-pointer" />
                                </TooltipTrigger>
                                <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                    <span>Fill new attribute</span>
                                </TooltipContent>
                            </Tooltip>
                        </div>
                    </DialogTrigger>
                    <Tooltip>
                    <TooltipTrigger asChild>
                    <img onClick={handleOpenEdit} src={editButtonIcon} alt="edit-button" 
                        className={`h-6 w-6 mt-0.5 ${selectedNames.length > 1 ? "opacity-45 pointer-events-none" : "cursor-pointer"}`} />
                    </TooltipTrigger>
                    <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                        <span>Edit attribute</span>
                    </TooltipContent>
                    </Tooltip>
                    <DialogContent ref={dialogContentRef} onOpenAutoFocus={(e) => e.preventDefault()} onPointerDownOutside={(e) => e.preventDefault()} className="h-160 w-230 sm:max-w-250 flex flex-col">
                        <DialogHeader className="flex items-center pt-2 mb-5">
                            <DialogTitle className="font-medium text-base font-sans">{editingMode ? "Edit attribute" : "Fill new attribute"}</DialogTitle>
                        </DialogHeader>

                        <div className="flex flex-col h-full justify-between gap-7">
                            <div className="flex flex-row gap-2">
                                <Combobox 
                                    items={attributes}
                                    value={attribute} 
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
                                        className="w-72" 
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
                            </div>
                            <div className="flex flex-col">

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
                                                <div className="border p-2 rounded-md bg-zinc-100 border-zinc-400 ">
                                                    <span key={index} className="text-[15px] font-sans">{option}</span>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            )}

                            </div>

                            <div className="mb-15 max-w-70">
                                <Label></Label>
                                <Input></Input>
                            </div>
                        </div>
                        <Button  disabled={isLoading}  className="w-28 absolute bottom-0 mb-4"onClick={editingMode ? editAttribute : createAttribute}>
                            {isLoading ? (<><Loader2 className="mr-2 h-4 w-4 animate-spin"/> Saving... </>) : "Save"}
                        </Button>
                    </DialogContent>
                </Dialog>
                <Tooltip>
                    <TooltipTrigger asChild>
                        <img onClick={deleteAttributes} src={deleteButtonIcon} alt="delete-button" className="w-7 h-7 cursor-pointer"/>
                    </TooltipTrigger>
                    <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                        <span>Delete new attribute</span>
                    </TooltipContent>
                    </Tooltip>
        </div>
        <Table className="min-w-full table-fixed">
            <TableHeader>
                <TableRow>
                    <TableHead className="w-7">
                    </TableHead>
                    <TableHead >Attribute name</TableHead>
                    <TableHead>Value</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {paginatedAttributesValues.map(([name, value]) => {
                const isSelected = selectedNames.includes(name);
                    return (
                        <TableRow key={name} onClick={() => toggleSelectedRow(name)} data-state={isSelected ? "selected" : ""}>
                            <TableCell>
                                <Checkbox checked={isSelected}/>
                            </TableCell>
                        <TableCell>{name}</TableCell>
                        <TableCell>{value}</TableCell>
                        </TableRow>
                    )}
                )}
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
        </>
    );
}
export default CandidatePanel;