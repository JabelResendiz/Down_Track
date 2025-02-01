import React, { useState, useEffect } from "react";
import {
  Card,
  CardHeader,
  CardBody,
  Typography,
  Button,
  IconButton,
} from "@material-tailwind/react";
import { ChevronLeftIcon, ChevronRightIcon } from "@heroicons/react/24/outline";
import api from "@/middlewares/api"; // Asegúrate de que api esté configurado correctamente

const EmployeesTable = () => {
  const [employeeList, setEmployeeList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const pageSize = 14;

  useEffect(() => {
    fetchEmployees(currentPage);
  }, [currentPage]);

  const fetchEmployees = async (pageNumber) => {
    setLoading(true);
    setError(null);
    try {
      const response = await api(
        `/Employee/GetPaged?PageNumber=${pageNumber}&PageSize=${pageSize}`,
        {
          method: "GET",
        },
      );

      if (!response.ok) {
        throw new Error("Failed to fetch employees");
      }

      const data = await response.json();
      setEmployeeList(data.items); // Ajusta la estructura si es necesario
      setTotalPages(Math.ceil(data.totalCount / pageSize)); // Calcula el total de páginas
    } catch (err) {
      console.error("Error fetching employees:", err);
      setError("Failed to load employee data");
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

    return Array.from(
      { length: endPage - startPage + 1 },
      (_, i) => startPage + i,
    ).map((page) => (
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

  return (
    <Card className="mt-8 shadow-lg">
      <CardHeader
        variant="gradient"
        color="gray"
        className="flex items-center justify-between p-6"
      >
        <Typography
          variant="h6"
          color="white"
          className="text-xl font-semibold"
        >
          Employees
        </Typography>
      </CardHeader>
      <CardBody className="px-0 py-4">
        {loading ? (
          <Typography className="text-center">Loading...</Typography>
        ) : error ? (
          <Typography color="red" className="text-center">
            {error}
          </Typography>
        ) : (
          <>
            {/* Employee Table */}
            <div className="overflow-x-auto">
              <table className="min-w-full table-auto text-sm text-gray-900">
                <thead className="bg-gray-800 text-white">
                  <tr>
                    <th className="border-b px-6 py-3 text-center">Username</th>
                    <th className="border-b px-6 py-3 text-center">Role</th>
                  </tr>
                </thead>
                <tbody className="bg-white">
                  {employeeList.length > 0 ? (
                    employeeList.map((user) => (
                      <tr key={user.id}>
                        <td className="border-b px-6 py-3 text-center">
                          {user.userRole.toLowerCase() == "shippingsupervisor"
                            ? user.name
                            : user.userName}
                        </td>
                        <td className="border-b px-6 py-3 text-center">
                          {user.userRole || "N/A"}
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="2" className="px-6 py-3 text-center">
                        No employees found
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </>
        )}

        {/* Paginación */}
        {!loading && !error && totalPages > 1 && (
          <div className="mt-4 flex justify-center space-x-2">
            {/* Previous button */}
            {currentPage > 1 && (
              <IconButton
                variant="outlined"
                color="gray"
                onClick={() => handlePageChange(currentPage - 1)}
                className="px-4 py-2"
              >
                <ChevronLeftIcon className="h-5 w-5" />
              </IconButton>
            )}

            {/* Page numbers */}
            {renderPaginationButtons()}

            {/* Next button */}
            {currentPage < totalPages && (
              <IconButton
                variant="outlined"
                color="gray"
                onClick={() => handlePageChange(currentPage + 1)}
                className="px-4 py-2"
              >
                <ChevronRightIcon className="h-5 w-5" />
              </IconButton>
            )}
          </div>
        )}
      </CardBody>
    </Card>
  );
};

export default EmployeesTable;
