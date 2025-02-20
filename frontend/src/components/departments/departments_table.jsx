import { useState, useEffect } from "react";
import { useLocation, useNavigate } from 'react-router-dom';
import { Card, CardHeader, CardBody, Typography, Button, IconButton } from "@material-tailwind/react";
import { ChevronLeftIcon, ChevronRightIcon,ArrowLeftIcon } from "@heroicons/react/24/outline";
import api from "@/middlewares/api";

export function DepartmentsTable() {
    const location = useLocation();
    const navigate = useNavigate();
    const { sectionId, sectionName } = location.state || {};
    
    const [departments, setDepartments] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const pageSize = 7;

    useEffect(() => {
        if (sectionId) {
            fetchDepartments(sectionId, currentPage);
        }
    }, [sectionId, currentPage]);

    const fetchDepartments = async (id, pageNumber) => {
        setLoading(true);
        setError(null);
        try {
            const response = await api(`/Department/GetAllDepartment_In_Section?sectionId=${sectionId}`, {
                method: "GET",
            });
            console.log(response)

            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            const data = await response.json();

            console.log(data)
            setDepartments(data);
            setTotalPages(Math.ceil(data.length / pageSize));
        } catch (error) {
            setError("Failed to fetch departments");
        } finally {
            setLoading(false);
        }
    };

    const handlePageChange = (pageNumber) => {
        if (pageNumber >= 1 && pageNumber <= totalPages) {
            setCurrentPage(pageNumber);
        }
    };

    const renderPaginationButtons = () => {
        const visibleButtons = 5;
        let startPage = Math.max(1, currentPage - Math.floor(visibleButtons / 2));
        let endPage = Math.min(totalPages, startPage + visibleButtons - 1);

        if (endPage - startPage + 1 < visibleButtons) {
            startPage = Math.max(1, endPage - visibleButtons + 1);
        }

        return Array.from({ length: endPage - startPage + 1 }, (_, i) => startPage + i).map((page) => (
            <Button
                key={page}
                variant={page === currentPage ? "filled" : "outlined"}
                color="gray"
                onClick={() => handlePageChange(page)}
                className="px-4 py-2"
            >
                {page}
            </Button>
        ));
    };

    const handleViewInventory = (departmentId) => {
        const _sectionId = null;
        navigate("inventory/", { state: { _sectionId, departmentId } });
    };

    return (
        <div className="mt-12 mb-8 flex flex-col gap-8">
            <Card>
                <CardHeader variant="gradient" color="gray" className="mb-8 p-6">
                    <div className="flex justify-between items-center">
                        <IconButton
                            variant="text"
                            color="white"
                            onClick={() => navigate(-1)}
                        >
                            <ArrowLeftIcon className="h-6 w-6" />
                        </IconButton>
                        <Typography variant="h6" color="white">
                            Departments in {sectionName}
                        </Typography>
                    </div>
                </CardHeader>
                <CardBody className="px-4 py-2">
                    {loading && <Typography variant="small" color="gray">Loading departments...</Typography>}
                    {error && <Typography variant="small" color="red">{error}</Typography>}
                    <div className="space-y-4">
                        {departments.map((department) => (
                            <div
                                key={department.id}
                                className="flex items-center justify-between border border-gray-200 rounded-lg shadow-sm p-4"
                            >
                                <Typography variant="h6" color="blue-gray">Department: {department.name}</Typography>
                                <div className="flex gap-2">
                                    <Button 
                                        size="sm" 
                                        color="gray"
                                        className="bg-gray-700 hover:bg-gray-500"
                                        onClick={() => handleViewInventory(department.id)}
                                    >
                                        View Inventory
                                    </Button>
                                </div>
                            </div>
                        ))}
                    </div>
                    <div className="flex justify-center mt-4 space-x-2">
                        {currentPage > 1 && (
                            <IconButton
                                variant="outlined"
                                size="sm"
                                color="gray"
                                onClick={() => handlePageChange(currentPage - 1)}
                                className="px-4 py-2"
                            >
                                <ChevronLeftIcon className="h-5 w-5" />
                            </IconButton>
                        )}
                        {/*renderPaginationButtons()*/}
                        {/* {currentPage < totalPages && (
                            <IconButton
                                variant="outlined"
                                size="sm"
                                color="gray"
                                onClick={() => handlePageChange(currentPage + 1)}
                                className="px-4 py-2"
                            >
                                <ChevronRightIcon className="h-5 w-5" />
                            </IconButton>
                        )} */}
                    </div>
                </CardBody>
            </Card>
        </div>
    );
}

DepartmentsTable.displayName = "/src/pages/departments_table.jsx";
export default DepartmentsTable;
