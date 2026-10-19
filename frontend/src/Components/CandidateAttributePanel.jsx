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
import Lightbox from "yet-another-react-lightbox";
import "yet-another-react-lightbox/styles.css"
import { api } from "../api/client";
import { toast } from "sonner";

const CandidatePanel = (id) => {
    const [attributes, setAttributes] = useState([]);
    const [attributeValues, setAttributeValues] = useState([]);
    const [attribute, setAttribute] = useState(null);
    const [attributePrefix, setAttributePrefix] = useState("");
    // const [attributeValueDictionary, setAttributeValueDictionary] = useState({});
    const [isModalOpen, setModalOpen] = useState(false);
    const [editingMode, setEditingMode] = useState(false);
    const [totalPages, setTotalPages] = useState(1);
    const [editingId, setEditingId] = useState("");
    const [userValue, setUserValue] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [previewUrl, setPreviewUrl] = useState(null);
    const [index, setIndex] = useState(-1);
    // const [selectedNames, setSelectedNames] = useState([]);
    const [selectedIds, setSelectedIds] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const dialogContentRef = useRef(null);
    const [filterCategory, setFilterCategory] = useState("");
    const [categories, setCategories] = useState([]);
    const [fileName, setFileName] = useState("");
    const itemPerPage = 6;
    const attributeNameMax = 26;
    const [version, setVersion] = useState(0);
    const startIndex = (currentPage - 1) * itemPerPage;
    const endIndex = startIndex + itemPerPage;
    const [file, setFile] = useState(null);
    const paginatedAttributesValues = attributeValues.slice(startIndex, endIndex);
    useEffect(() => {
        getAttributeValues();
        getCategories();
    }, []);


    useEffect(() => {
        getAttributesData();
    }, [attributePrefix, filterCategory, isModalOpen]);


    const dragPhoto = () => {
        setLoading("Uploading...");

    }

    const handleOpenEdit = async () => {
        if (selectedIds.length !== 1) return;
        const targetId = selectedIds[0];
        const attributeValue = attributeValues.find(attr => attr.id === targetId);
        if (!attributeValue) return;
        console.log(attributeValue);
        setEditingMode(true);
        setEditingId(attributeValue.id);
        setUserValue(attributeValue.value);
        setVersion(attributeValue.version);
        const fullAttribute = await getAttributeById(attributeValue.attributeId);
        if (fullAttribute) {
            setAttribute(fullAttribute);
            setAttributePrefix(fullAttribute.name);
        } else {
            setAttributePrefix(attributeValue.attributeName);
        }
        setModalOpen(true);
    };

    const toggleSelectedRow = (id) => {
        setSelectedIds(prev => prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]);
    }

    const getAttributesData = async () => {
        const params = new URLSearchParams();
        if (attributePrefix.trim() !== "") {
            params.append("prefix", attributePrefix.trim());
        }
        if (filterCategory && filterCategory.trim() !== "") {
            params.append("category", filterCategory.trim());
        }
        params.append("page", 1);
        params.append("pageSize", itemPerPage);

        const { data, error } = await api.get(`/api/attribute/attributes?${params.toString()}`);
        if (error) return;
        
        setAttributes(data.attributeResponseDTO);
    }

    const getAttributeById = async (id) => {
        const { data, error } = await api.get(`/api/attribute/attribute?id=${id}`);
        if (error) return null;
        
        setAttribute(data);
        return data; 
    }

    const getAttributeValues = async () => {
        const { data, error } = await api.get(`/api/attribute/candidate-attributes?userId=${id.id}`);
        if (error) return;

        setAttributeValues(data);
        setTotalPages(Math.ceil(data.length / itemPerPage));
    }

    const getCategories = async () => {
        const { data, error } = await api.get(`/api/attribute/categories`);
        if (error) return;
        
        setCategories(data);
    }

    const saveAttributeImage = async (file) => {
        const formData = new FormData();
        formData.append("AttributeId", attribute.id);
        formData.append("file", file);
        
        setIsLoading(true);
        const toastId = toast.loading("Saving...");

        const { error } = await api.post("/api/attribute/candidate-attribute-image", formData);
        
        setIsLoading(false);

        if (error) {
            toast.dismiss(toastId);
            return;
        }

        toast.success("Saved", { id: toastId });
        setPreviewUrl("");
        setFileName("");
        getAttributeValues();
        setFile(null);
        setModalOpen(false);
    }

    const createAttribute = async () => {
        setIsLoading(true);
        const toastId = toast.loading("Saving...");

        const { error } = await api.post("/api/attribute/candidate-attribute", {
            AttributeId: attribute.id,
            Value: userValue
        });

        setIsLoading(false);

        if (error) {
            toast.dismiss(toastId);
            return;
        }

        toast.success("Saved successfully", { id: toastId });
        getAttributeValues();
        resetForm();
        setModalOpen(false);
    }

    const deleteAttributes = async () => {
        const toastId = toast.loading("Deleting...");

        const { error } = await api.delete("/api/attribute/candidate-attributes", selectedIds);

        if (error) {
            toast.dismiss(toastId);
            return;
        }

        toast.success("Attributes deleted", { id: toastId });
        getAttributeValues();
        setSelectedIds([]);
    }
    
    const saveLocalFile = (File) => {
        setFileName(File.name);
        setPreviewUrl(URL.createObjectURL(File));
        setFile(File);
    }

    const editAttribute = async (attributeId) => {
        const toastId = toast.loading("Saving...");
        const {data, error} = await api.put("/api/attribute/candidate-attribute", {
            AttributeId: attributeId,
            Value: userValue,
            Version: version
        });
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved successfully", { id: toastId });
        getAttributeValues();
        resetForm();
    }

    const resetForm = () => {
        setUserValue("");
        setAttributePrefix("");
        setFilterCategory("");
        setSelectedIds([]);
        setVersion(0);
        setModalOpen(false);
        setAttribute(null);
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
                        className={`h-6 w-6 mt-0.5 ${selectedIds.length > 1 ? "opacity-45 pointer-events-none" : "cursor-pointer"}`} />
                    </TooltipTrigger>
                    <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                        <span>Edit attribute</span>
                    </TooltipContent>
                    </Tooltip>
                    <DialogContent ref={dialogContentRef} onOpenAutoFocus={(e) => e.preventDefault()} onPointerDownOutside={(e) => e.preventDefault()} className="h-130 w-150 sm:max-w-250 flex flex-col">
                        <DialogHeader className="flex items-center pt-2 mb-5">
                            <DialogTitle className="font-medium text-base font-sans">{editingMode ? "Edit attribute" : "Fill new attribute"}</DialogTitle>
                        </DialogHeader>

                        <div className="flex flex-col h-full  gap-7">
                            <div className="flex flex-row gap-5">
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
                                                <div className="border p-1 rounded-md bg-zinc-100 border-zinc-400 ">
                                                    <span key={index} className="text-[15px] font-sans">{option}</span>
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            )}

                            { attribute && (
                                attribute.dataType != "Image" ? (
                                <div className="mb-15 max-w-72">
                                    <Input placeholder="Enter your value" value={userValue} onChange={(e) => setUserValue(e.target.value)} ></Input>
                                </div>) : (<>
                                <Dropzone onDrop={
                                    (acceptedFiles, rejectedFiles) => {
                                        if(rejectedFiles.length > 0){
                                            console.log("You can insert only one file");
                                            return;
                                        }
                                        if(acceptedFiles.length > 0){
                                            const file = acceptedFiles[0];
                                            saveLocalFile(file);
                                        }
                                    }
                                } maxFiles={1}>
                                    {
                                        ({getRootProps, getInputProps}) => (
                                            fileName.trim() === "" ?
                                            <div {...getRootProps()} className="border-2 border-dashed
                                             border-gray-300 rounded-xl p-8 text-center cursor-pointer 
                                             hover:border-gray-400 transition-colors flex flex-col items-center max-w-40 justify-center max-h-30"> 
                                                <Input {...getInputProps()} />
                                                <p className="text-sm text-gray-600 font-medium">Drag your files here or click</p>
                                                <span className="text-sm text-gray-600 font-medium">Supported formats JPG, PNG, WEBP</span>
                                            </div> :
                                            <div onClick={() => setIndex(0)} className="border-2 hover:cursor-pointer rounded-md w-fit gap-1.5 inline-flex items-center hover:border-zinc-300 transition-colors text-sm p-2">
                                                <span>{fileName}</span>
                                                <button type="button" onClick={(e) => {e.stopPropagation(); setFileName(""); setPreviewUrl(null);}} className="text-zinc-400 hover:text-zinc-800 transition-colors" >×</button>
                                            </div>
                                        ) 
                                    }
                                </Dropzone>
                                    </>)
                                )
                            }

                            </div>
                        </div>
                        <Button className="w-28 absolute bottom-0 mb-4"onClick={attribute?.dataType === "Image" ? () => saveAttributeImage(file) : ( editingMode ? () => editAttribute(attribute.id) : createAttribute) }>
                            <span> Save </span>
                        </Button>
                    </DialogContent>
                </Dialog>
                <Lightbox carousel={{ finite: true }} render={{ buttonPrev: () => null, buttonNext: () => null }} open={index >= 0} close={() => setIndex(-1)} index={index} slides={previewUrl ? [{ src: previewUrl }] : []} ></Lightbox>
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
                {paginatedAttributesValues.map((item) => {
                const isSelected = selectedIds.includes(item.id);
                    return (
                        <TableRow key={item.id} onClick={() => toggleSelectedRow(item.id)} data-state={isSelected ? "selected" : ""}>
                            <TableCell>
                                <Checkbox checked={isSelected}/>
                            </TableCell>
                        <TableCell>
                            {item.attributeName.length < 30 ? (
                                item.attributeName
                            ) : (
                                <Tooltip>
                                    <TooltipTrigger className="text-left">
                                        {item.attributeName.slice(0, attributeNameMax) + "..."}
                                    </TooltipTrigger>
                                    <TooltipContent>
                                        {item.attributeName}
                                    </TooltipContent>
                                </Tooltip>
                            )}
                        </TableCell>
                        <TableCell>{item.dataType != 2 ? item.value : <span onClick={() => {setPreviewUrl(item.value); setIndex(0); toggleSelectedRow(item.id)}} className="underline cursor-pointer hover:text-zinc-600">View image</span>}</TableCell>
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