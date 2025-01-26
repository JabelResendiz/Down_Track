using System.Linq.Expressions;
using AutoMapper;
using DownTrack.Application.DTO;
using DownTrack.Application.DTO.Paged;
using DownTrack.Application.IServices;
using DownTrack.Application.IUnitOfWorkPattern;
using DownTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DownTrack.Application.IRepository;

namespace DownTrack.Application.Services;


/// <summary>
/// Service class for handling business logic related to departments.
/// Interacts with repositories and uses DTOs for client communication.
/// </summary>
public class DepartmentServices : IDepartmentServices
{

    // Automapper instance for mapping between domain entities and DTOs.
    private readonly IMapper _mapper;

    // Unit of Work instance for managing repositories and transactions.
    private readonly IUnitOfWork _unitOfWork;

    private readonly IGenericRepository<Department> _departmentRepository;
    public DepartmentServices(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Department> departmentRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _departmentRepository = departmentRepository;
    }


    /// <summary>
    /// Creates a new department based on the provided DTO.
    /// </summary>
    /// <param name="dto">The DepartmentDto containing the department details to create.</param>
    /// <returns>The created DepartmentDto.</returns>
    public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
    {

        //Maps DTO to domain entity.

        var department = _mapper.Map<Department>(dto);

        var departmentRepository = _unitOfWork.DepartmentRepository;

        bool existDepartment = await departmentRepository
                                    .ExistsByNameAndSectionAsync(department.Name, department.SectionId);

        if (existDepartment)
            throw new Exception("A department with the same name already exists in this section.");

        department.Section = await _unitOfWork.GetRepository<Section>().GetByIdAsync(dto.SectionId);

        //Adds the new department to the repository.
        await _unitOfWork.GetRepository<Department>().CreateAsync(department);

        //Commits the transaction.
        await _unitOfWork.CompleteAsync();

        // Maps the created entity back to DTO.
        return _mapper.Map<DepartmentDto>(department);

    }

    // public async Task DeleteAsync(int departmentId, int sectionId)
    // {

    //     await _unitOfWork.DepartmentRepository.DeleteAsync(departmentId, sectionId);

    //     await _unitOfWork.CompleteAsync();
    // }



    /// <summary>
    /// Deletes a department by its ID.
    /// </summary>
    /// <param name="dto">The ID of the department to delete.</param>
    public async Task DeleteAsync(int dto)
    {
        // Removes the department by its ID
        await _unitOfWork.GetRepository<Department>().DeleteByIdAsync(dto);

        await _unitOfWork.CompleteAsync(); // Commits the transaction.
    }

    public async Task<IEnumerable<DepartmentDto>> ListAsync()
    {
        var departments = await _unitOfWork
                                .GetRepository<Department>()
                                .GetAll()
                                .Include(d => d.Section) // Load the relation Section
                                .ToListAsync(); // List the values

        return departments.Select(_mapper.Map<DepartmentDto>);
    }



    /// <summary>
    /// Updates an existing department's information.
    /// </summary>
    /// <param name="dto">The DepartmentDto containing updated details.</param>
    /// <returns>The updated DepartmentDto.</returns>
    public async Task<DepartmentDto> UpdateAsync(DepartmentDto dto)
    {

        var existingDepartment = await _unitOfWork.GetRepository<Department>().GetByIdAsync(dto.Id);

        if (existingDepartment == null)
        {
            throw new ConflictException($"Department with ID '{dto.Id}' in section '{dto.SectionId}' does not exist.");
        }

        var existingSection = await _unitOfWork.GetRepository<Section>().GetByIdAsync(dto.SectionId);

        if (existingSection == null)
            throw new ConflictException($"Section '{dto.SectionId}' does not exist.");

        _mapper.Map(dto, existingDepartment);

        _unitOfWork.GetRepository<Department>().Update(existingDepartment);

        await _unitOfWork.CompleteAsync();

        return _mapper.Map<DepartmentDto>(existingDepartment);

    }



    /// <summary>
    /// Retrieves a department by its ID.
    /// </summary>
    /// <param name="departmentDto">The ID of the department to retrieve.</param>
    /// <returns>The DepartmentDto of the retrieved department.</returns>
    public async Task<DepartmentDto> GetByIdAsync(int departmentDto)
    {

        var filter = new List<Expression<Func<Department, bool>>>()
        {
            d=> d.Id == departmentDto
        };

        var result = await _unitOfWork.GetRepository<Department>()
                                      .GetAllByItems(filter)
                                      .Include(d => d.Section)
                                      .ToListAsync();


        return _mapper.Map<DepartmentDto>(result);

    }

    public async Task<PagedResultDto<DepartmentDto>> GetPagedResultAsync(PagedRequestDto paged)
    {
        //The queryable collection of entities to paginate
        IQueryable<Department> queryDepartment = _unitOfWork.GetRepository<Department>().GetAll();

        var totalCount = await queryDepartment.CountAsync();

        var items = await queryDepartment // Apply pagination to the query.
                        .Skip((paged.PageNumber - 1) * paged.PageSize) // Skip the appropriate number of items based on the current page
                        .Take(paged.PageSize) // Take only the number of items specified by the page size.
                        .ToListAsync(); // Convert the result to a list asynchronously.


        return new PagedResultDto<DepartmentDto>
        {
            Items = items?.Select(_mapper.Map<DepartmentDto>) ?? Enumerable.Empty<DepartmentDto>(),
            TotalCount = totalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            NextPageUrl = paged.PageNumber * paged.PageSize < totalCount
                        ? $"{paged.BaseUrl}?pageNumber={paged.PageNumber + 1}&pageSize={paged.PageSize}"
                        : null,
            PreviousPageUrl = paged.PageNumber > 1
                        ? $"{paged.BaseUrl}?pageNumber={paged.PageNumber - 1}&pageSize={paged.PageSize}"
                        : null

        };
    }


    public async Task<PagedResultDto<Department>> GetPagedDepartmentsBySectionIdAsync(
    int sectionId,
    PagedRequestDto pagedRequest)
    {
        // Crear el filtro para departamentos de la sección específica
        var parameter = Expression.Parameter(typeof(Department), "department");
        var body = Expression.Equal(
            Expression.Property(parameter, "SectionId"),
            Expression.Constant(sectionId)
        );
        var filterExpression = Expression.Lambda<Func<Department, bool>>(body, parameter);

        // Aplicar el filtro al repositorio
        var query = _departmentRepository.GetAllByItems(new[] { filterExpression });

        // Obtener el número total de registros
        var totalRecords = query.Count();

        // Aplicar paginación
        var pagedItems = await query
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync();

        // Construir URLs para las páginas siguiente y anterior
        var nextPageUrl = totalRecords > pagedRequest.PageNumber * pagedRequest.PageSize
            ? $"{pagedRequest.BaseUrl}?pageNumber={pagedRequest.PageNumber + 1}&pageSize={pagedRequest.PageSize}"
            : null;

        var previousPageUrl = pagedRequest.PageNumber > 1
            ? $"{pagedRequest.BaseUrl}?pageNumber={pagedRequest.PageNumber - 1}&pageSize={pagedRequest.PageSize}"
            : null;

        // Crear el resultado paginado
        var result = new PagedResultDto<Department>
        {
            Items = pagedItems,
            TotalCount = totalRecords,
            PageNumber = pagedRequest.PageNumber,
            PageSize = pagedRequest.PageSize,
            NextPageUrl = nextPageUrl,
            PreviousPageUrl = previousPageUrl
        };

        return result;
    }


}