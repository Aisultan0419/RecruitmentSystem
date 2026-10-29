import {useEffect, useState, useRef} from 'react';
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/Components/ui/table"
import {Tooltip, TooltipContent, TooltipTrigger } from "@/Components/ui/tooltip"
import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious, } from "@/Components/ui/pagination" 
import createButtonIcon from "./assets/create-button.svg";
import editButtonIcon from "./assets/edit-button.svg";
import { Input } from "@/Components/ui/input";
import { Label } from "@/Components/ui/label";
import { Button } from "@/Components/ui/button";
import { Checkbox } from "@/Components/ui/checkbox";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue, } from "@/Components/ui/select"
import deleteButtonIcon from "./assets/delete-button.svg";
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList, } from "@/Components/ui/combobox"
import { Dialog, DialogTrigger, DialogContent, DialogTitle, DialogHeader} from "@/Components/ui/dialog"
import { ShieldCheck, Shield, ChevronDown, XCircle } from "lucide-react";
import { api } from "../api/client";
import { toast } from "sonner";

function useDebounce(value, delay = 700){
    const [debouncedValue, setDebouncedValue] = useState(value)

    useEffect(() => {
        const handler = setTimeout(() => setDebouncedValue(value), delay);
        return () => clearTimeout(handler);
    }, [value, delay])

    return debouncedValue;
}

const PositionPanel = () => {

    const [selectedIds, setSelectedIds] = useState([]);
    const [isModalOpen, setModalOpen] = useState(false);
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [searchTitle, setSearchTitle] = useState("");
    const [searchDescription, setSearchDescription] = useState("");
    const [isDraft, setDraft] = useState(null);
    const [isNewestFirst, setIsNewestFirst] = useState(true);
    const [maxProjectsCount, setMaxProjectsCount] = useState(0);
    const [attributes, setAttributes] = useState([]);
    const [attribute, setAttribute] = useState();
    const dialogContentRef = useRef(null);
    const [positions, setPositions] = useState([]);
    const [attributePrefix, setAttributePrefix] = useState("");
    const [filterCategory, setFilterCategory] = useState("");
    const [categories, setCategories] = useState([]);
    const [version, setVersion] = useState(0);
    const [positionId, setPositionId] = useState(null);
    const [pickedAttributes, setPickedAttributes] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const itemPerPage = 11;
    const maxTitleLength = 40;
    const maxDescriptionLength = 40;    
    const [accessRules, setAccessRules] = useState([]);
    const [totalPages, setTotalPages] = useState();
    const debouncedSearchTitle = useDebounce(searchTitle, 500);
    const debouncedSearchDescription = useDebounce(searchDescription, 500);
    const isDirtyRef = useRef(false);
    const isDirtyAccessRuleRef = useRef(false);
    const [accessRuleValue, setAccessRuleValue] = useState("");
    const [secondAccessRuleValue, setSecondAccessRuleValue] = useState("");
    const [accessRuleId, setAccessRuleId] = useState();
    const [operator, setOperator] = useState();
    const operators = [
        "None",
        "Equals",
        "NotEquals",
        "Greater",
        "Less",
        "GreaterOrEqual",
        "LessOrEqual",
        "Contains",
        "NotContains",
        "StartsWith",
        "EndsWith",
        "Between",
    ];
    const [expandedRowId, setExpandedRowId] = useState();

    const toggleSelectedRow = (id) => {
        setSelectedIds(prev => prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id])
    }
    useEffect(() => {
        getCategories();
    }, []);

    useEffect(() => {
        if(!isDirtyAccessRuleRef.current) return;

        if(operator === undefined || accessRuleValue === ""){
            return;
        }
        const timer = setTimeout(() => {
            if (accessRuleId.trim() === "") {
                addAccessRule();
            }
            else if(operator.trim() === "" && accessRuleValue.trim() === ""){
                deleteAccessRule();
            } 
            else {
                editAccessRule();
            }
        }, 1000);

        return () => clearTimeout(timer);
    }, [operator, accessRuleValue, secondAccessRuleValue]);

    useEffect(() => {
            getAttributesData();
        }, [attributePrefix, filterCategory, isModalOpen]);

    useEffect(() => {
        getPositions();
    }, [currentPage, debouncedSearchTitle, debouncedSearchDescription, isNewestFirst, isDraft]);

    useEffect(() => {
    if (!isDirtyRef.current) return;

    if (!title.trim() || !description.trim()) {
        return;
    }

    const timer = setTimeout(() => {
        if (positionId === null) {
            createPosition();
        } 
        else {
            updatePosition();
        }
    }, 1000);

    return () => clearTimeout(timer);
    }, [title, description, maxProjectsCount, isDirtyRef, positionId]);

    const getPositionAttributes = async (positionId) => {
        const {data, error} = await api.get(`/api/position/positionAttributes?positionId=${positionId}`)

        if(error){
            return;
        }
        console.log(data);
        setPickedAttributes(data);
    }

    const publish = async () => {
        const toastId = toast.loading("Saving...");
        const {error} = await api.put(`/api/position/publish?positionId=${positionId}`);
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        resetForm();
        getPositions();
    }

    const unPublish = async () => {
        const toastId = toast.loading("Saving...");
        const {error} = await api.put(`/api/position/unPublish?positionId=${positionId}`);
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        resetForm();
        getPositions();
    }


    const getAccessRule = async (positionId) => {
        const {data, error} = await api.get(`/api/position/positionAccessRules?positionId=${positionId}`);
        if(error) return;
        setAccessRules(data);
    }

    const deleteAccessRule = async () => {
        const toastId = toast.loading("Saving...");
        const {error} = await api.delete(`/api/position/delete?positionAccessRuleId=${accessRuleId}`);
        if(error) {
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        setExpandedRowId();
        getAccessRule();
    }

    const editAccessRule = async () => {
        const toastId = toast.loading("Saving...");
        const params = new URLSearchParams({
            positionAccessRuleId: accessRuleId,
            value: accessRuleValue,
            comparisonOperator: operator
        });

        if (secondAccessRuleValue && secondAccessRuleValue.trim() !== "") {
            params.append("secondValue", secondAccessRuleValue.trim());
        }

        const { error } = await api.put(`/api/position/access-rule?${params.toString()}`);
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        getAccessRule(positionId);
    }

    const addAccessRule = async () => {
        const toastId = toast.loading("Saving...")
        const {error} = await api.post("/api/position/access-rule", {
            AttributeId: expandedRowId,
            PositionId: positionId,
            ComparisonOperator: operator,
            Value: accessRuleValue, 
            SecondValue: secondAccessRuleValue.trim() === "" ? undefined : secondAccessRuleValue
        });
        console.log(expandedRowId)
        console.log(positionId)
        console.log(operator)
        console.log(accessRuleValue)
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Saved", {id: toastId});
        getAccessRule(positionId);
    }

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

    const deletePositions = async () => {
        const toastId = toast.loading("Saving...")
        const {error} = await api.delete("/api/position/positions", selectedIds);
        if(error){
            toast.dismiss(toastId);
            return;
        }
        toast.success("Selected positions deleted", { id: toastId });
        setSelectedIds([]);
        getPositions();
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

    const getPositions = async () => {
        const params = new URLSearchParams();
        if (debouncedSearchTitle?.trim()) {
            params.append("Title", debouncedSearchTitle.trim());
        }
        if (debouncedSearchDescription?.trim()) {
            params.append("Description", debouncedSearchDescription.trim());
        }
        if(isDraft != null){
            params.append("IsDraft", isDraft);
        }
        params.append("Page", currentPage);
        params.append("PageSize", itemPerPage);
        params.append("NewestFirst", isNewestFirst);

        const {data, error} = await api.get(`/api/position?${params.toString()}`)

        if(error){
            return;
        }
        setTotalPages(Math.ceil(data.totalItems / itemPerPage));
        setPositions(data.positionResponseDTO);
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
        getPositions();
    }

    const expandRow = (optionId) => {
        if (expandedRowId === optionId) {
            setExpandedRowId(null);
            setAttribute(null);
            setAccessRuleId("");
            setOperator(undefined);
            setAccessRuleValue("");
            setSecondAccessRuleValue("");
            isDirtyAccessRuleRef.current = false;
        }
        else {
            const accessRule = accessRules.find(ac => ac.attributeDefinitionId === optionId );
            if(accessRule !== undefined){
                const matchedOperator = operators.find(op => op.toLowerCase() === accessRule.operator.toLowerCase());
                setOperator(matchedOperator || undefined);
                setAccessRuleValue(accessRule.value ?? "");
                setAccessRuleId(accessRule.id);
                setSecondAccessRuleValue(accessRule.secondValue ?? "");
            }
            const chosenAttribute = pickedAttributes.find(at => at.id === optionId);
            if(chosenAttribute !== undefined){
                setAttribute(chosenAttribute);
            }
            setExpandedRowId(optionId);
        }
    }

    const handleOpenEdit = () => {
        if (selectedIds.length !== 1) return;

        const targetId = selectedIds[0];
        const position = positions.find(
            position => position.id === targetId
        );

        if (position) {
            isDirtyRef.current = false;
            setPositionId(position.id);
            getAccessRule(position.id);
            setTitle(position.title);
            setDescription(position.description);
            setMaxProjectsCount(position.maxProjectsCount);

            getPositionAttributes(position.id);
            setModalOpen(true);
        }
    };

    const getStatus = () => {
        const position = positions.find(p => p.id === positionId);

        if (!position || position.isDraft) {
            return "Publish";
        }

        return "Unpublish";
    };

    const resetForm = () => {
        setAttributePrefix("");
        setFilterCategory("");
        setAttribute(null);
        setModalOpen(false);
        setSelectedIds([]);
        setVersion(0);
        setPositionId(null);
        setExpandedRowId();
        setTitle("");
        setAccessRules([]);
        isDirtyRef.current = false;
        setPickedAttributes([]);
        setDescription("");
        setAccessRuleId("");
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
                                            <img src={createButtonIcon} alt="create-button" className={`w-6.5 h-6.5 ${selectedIds.length > 0 ? "opacity-45 pointer-events-none" : "cursor-pointer"}`} />
                                        </TooltipTrigger>
                                        <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                            <span>Create new position</span>
                                        </TooltipContent>
                                    </Tooltip>
                                </div>
                            </DialogTrigger>
                            <Tooltip> 
                                <TooltipTrigger asChild>
                                    <img onClick={handleOpenEdit} className={`h-7 w-7 ${selectedIds.length > 1 ? "opacity-45 pointer-events-none" : "cursor-pointer"}`} src={editButtonIcon} alt="edit-button" />
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
                                            <Input type="text" value={title} onChange={e => {setTitle(e.target.value); isDirtyRef.current = true;}} className="max-w-100"></Input>
                                            <label className="text-small text-zinc-600 font-medium">Description</label>
                                            <Input type="text" value={description} onChange={e => { setDescription (e.target.value);  isDirtyRef.current = true;}} className="max-w-100"></Input>
                                            <label className="text-small text-zinc-600 font-medium">Maximum projects number</label>
                                            <Input type="number" value={maxProjectsCount} onChange={e => {setMaxProjectsCount(e.target.value);  isDirtyRef.current = true;}} min={0} className="max-w-100"></Input>
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
                                                    {(attribute && !expandedRowId) && <Button onClick={() => positionId === null ? console.log("position is not created") : addAttribute(attribute, positionId)}>Add</Button>}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="flex flex-col w-1/2 flex-none items-center"> {/* Right column: attributes */}
                                        <span className="text-base">Chosen attributes</span>
                                        <div className="flex flex-row flex-wrap w-full min-w-0 overflow-y-auto max-h-100 mt-2">
                                            {pickedAttributes.length !== 0 ? (
                                                pickedAttributes.map((option, index) => {
                                                    const isExpanded = expandedRowId === option.id;
                                                    const hasRule = accessRules.some((rule) => rule.attributeDefinitionId === option.id);

                                                    return (
                                                        <div key={option.id} className="w-full border-b border-zinc-200 transition-colors bg-white rounded-md overflow-hidden">
                                                            {/*header row */}
                                                            <div className="w-full h-10 flex items-center justify-between px-2 cursor-pointer hover:bg-zinc-50 transition-colors "
                                                                 onClick={() => expandRow(option.id)}>
                                                                <div className="flex items-center gap-2 truncate">

                                                                    <div className="relative w-4 h-4 shrink-0 flex items-center justify-center">
                                                                        <Shield
                                                                            className={`absolute w-4 h-4 text-zinc-300 transition-all duration-300 ease-out ${
                                                                                hasRule ? "opacity-0 scale-50 rotate-45" : "opacity-100 scale-100 rotate-0"
                                                                            }`}
                                                                        />
                                                                        <ShieldCheck
                                                                            className={`absolute w-4 h-4 text-green-600 transition-all duration-300 ease-out ${
                                                                                hasRule ? "opacity-100 scale-100 rotate-0" : "opacity-0 scale-50 -rotate-45"
                                                                            }`}
                                                                        />
                                                                    </div>
                                                                    <span className="font-medium text-[14px] truncate">
                                                                        {option.name}
                                                                    </span>
                                                                </div>
                                                                <div className="flex items-center shrink-0">
                                                                    <ChevronDown className={`w-4 h-4 text-zinc-500 transition-transform duration-200 ${isExpanded ? "rotate-180" : ""}`}/>
                                                                    <button
                                                                        type="button"
                                                                        className="cursor-pointer ml-1 text-zinc-400 hover:text-zinc-700 p-1"
                                                                        onClick={(e) => {
                                                                            e.stopPropagation();
                                                                            deleteAttribute(option, positionId);
                                                                        }}
                                                                        >
                                                                        ×
                                                                    </button>
                                                                </div>
                                                            </div>
                                                            {/* expanded row */}
                                                            <div className={`grid transition-all duration-300 ease-in-out ${isExpanded ? "grid-rows-[1fr] opacity-100" : "grid-rows-[0fr] opacity-0"}`}>
                                                                <div className="overflow-hidden">
                                                                    <div className="p-3 bg-zinc-50 border-t-2 border-zinc-100 flex flex-col gap-2">
                                                                        <Select value={operator || ""} onValueChange={(val) => {setOperator(val); isDirtyAccessRuleRef.current = true;}}>
                                                                            <SelectTrigger className="bg-white w-full text-[14px]">
                                                                                <SelectValue placeholder="Select operator" />
                                                                            </SelectTrigger>
                                                                            <SelectContent>
                                                                                <SelectItem 
                                                                                    value="" 
                                                                                    className="text-zinc-500 font-medium cursor-pointer focus:bg-zinc-100 focus:text-zinc-800 flex items-center gap-2">
                                                                                    <XCircle className="w-4 h-4 text-zinc-400" />
                                                                                    <span>Clear selection</span>
                                                                                </SelectItem>
                                                                                {operators.map((op) => (
                                                                                <SelectItem key={op} value={op}>
                                                                                    {op}
                                                                                </SelectItem>
                                                                                ))}
                                                                            </SelectContent>
                                                                        </Select>
                                                                        <Input type="text" placeholder="Value..." className="bg-white w-full " value={accessRuleValue}
                                                                        onChange={(e) => { setAccessRuleValue(e.target.value), isDirtyAccessRuleRef.current = true;} }
                                                                        />

                                                                        <Input type="text" placeholder="Second value (optional)..." className="bg-white w-full" value={secondAccessRuleValue}
                                                                        onChange={(e) => { setSecondAccessRuleValue(e.target.value); isDirtyAccessRuleRef.current = true;} }
                                                                        />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                         </div>
                                                    );
                                                })) : (<span className="text-zinc-600 text-center mt-4"> No attributes chosen yet </span>)
                                            }
                                        </div>
                                    </div>
                                </div>
                                <div className="max-w-100 absolute bottom-0 mb-4 flex items-center mt-auto gap-6">
                                    <Button disabled={positionId === null} onClick={getStatus() === "Publish" ? publish : unPublish} className="w-25">{getStatus()}</Button>
                                </div>
                            </DialogContent>
                        </Dialog>
                        <Tooltip>
                            <TooltipTrigger asChild>
                                <img onClick={deletePositions} src={deleteButtonIcon} alt="delete-button" className="w-8 h-8 cursor-pointer"/>
                            </TooltipTrigger>
                            <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                <span>Delete positions</span>
                            </TooltipContent>
                        </Tooltip>

                        <div className="flex flex-col max-w-90 gap-2">
                            <Input type="text" placeholder="Search title..." value={searchTitle} onChange={(e) => setSearchTitle(e.target.value)} />
                        </div>
                        <div className="max-w-90"> 
                            <Input type="text" placeholder="Description..." value={searchDescription} onChange={(e) => setSearchDescription(e.target.value)} />
                        </div>
                        <Select value={isDraft} onValueChange={setDraft}>
                        <SelectTrigger className="w-40">
                            <SelectValue placeholder="Is draft" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value={null}>All</SelectItem>
                            <SelectItem value={true}>Draft</SelectItem>
                            <SelectItem value={false}>Published</SelectItem>
                        </SelectContent>
                     </Select>
                     <Select value={isNewestFirst} onValueChange={setIsNewestFirst}>
                        <SelectTrigger className="w-40">
                            <SelectValue placeholder="Newest first" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value={true}>Newest first</SelectItem>
                            <SelectItem value={false}>Oldest first</SelectItem>
                        </SelectContent>
                     </Select>
                    </div>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead className="w-12.5">
                                </TableHead>
                                <TableHead >Title</TableHead>
                                <TableHead>Description</TableHead>
                                <TableHead>IsDraft</TableHead>
                                <TableHead>Tags</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                        {positions.map((position) => {
                            const isSelected = selectedIds.includes(position.id);
                            return (<TableRow key={position.id} 
                            onClick={() => toggleSelectedRow(position.id)}
                            className="cursor-pointer transition-colors"
                            data-state={isSelected ? "selected" : ""}>
                            <TableCell className="w-12.5">
                            <Checkbox 
                                checked={isSelected}
                            />
                            </TableCell>
                            <TableCell className="max-w-50">
                            { position.title.length > maxTitleLength ? 
                                (
                                <Tooltip>
                                    <TooltipTrigger>
                                        <span>{position.title.slice(0,maxTitleLength)}...</span>
                                    </TooltipTrigger>
                                    <TooltipContent className="max-w-sm max-h-60 overflow-y-auto whitespace-pre-wrap wrap-break-words border-2 bg-white p-3 text-black font-sans font-medium shadow-md">
                                        <span>{position.title}</span>
                                    </TooltipContent>
                                </Tooltip>) : <span>{position.title}</span>
                            }
                            </TableCell>
                            <TableCell className="max-w-50">
                                { position.description.length > maxDescriptionLength ? 
                                (
                                    <Tooltip>
                                        <TooltipTrigger>
                                            <span>{position.description.slice(0,maxDescriptionLength)}...</span>
                                        </TooltipTrigger>
                                        <TooltipContent className="max-w-sm max-h-60 overflow-y-auto whitespace-pre-wrap wrap-break-words border-2 bg-white p-3 text-black font-sans font-medium shadow-md">
                                            <span>{position.description}</span>
                                        </TooltipContent>
                                    </Tooltip>) : <span>{position.description}</span>
                                }
                            </TableCell>
                            <TableCell>{position.isDraft ? "Yes" : "No"}</TableCell>
                            <TableCell className="text-right cursor-pointer hover:text-zinc-600"> 
                            { position.tags.length > 0 ? 
                                (<Tooltip>
                                    <TooltipTrigger>
                                        <span className="hover:text-zinc-700">{position.tags.length} tags</span>
                                    </TooltipTrigger>
                                    <TooltipContent className="font-medium font-sans bg-white text-black border-2">
                                        <ul>
                                            {position.tags.map((tag, i) => (
                                                <li key={i}>• {tag}</li>
                                            ))}
                                        </ul>
                                    </TooltipContent>
                                </Tooltip> ) : "—" }
                            </TableCell>
                             </TableRow>);
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
        </>
    );
}
export default PositionPanel;   