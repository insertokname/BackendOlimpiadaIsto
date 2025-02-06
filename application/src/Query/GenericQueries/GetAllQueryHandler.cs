using BackendOlimpiadaIsto.domain.Entities;
using BackendOlimpiadaIsto.infrastructure.Repositories;

namespace BackendOlimpiadaIsto.application.Query.GenericQueries;


public class GetAllQueryHandler<E>
where E : Entity
{
    private readonly IRepository<E> _entityRepository;

    public GetAllQueryHandler(IRepository<E> entityRepository)
    {
        _entityRepository = entityRepository;
    }

    public async Task<IEnumerable<E>> HandleAsync()
    {
        return await _entityRepository.GetAllAsync();
    }
}