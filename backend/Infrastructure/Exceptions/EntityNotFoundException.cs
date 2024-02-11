namespace McHelper.Infrastructure.Exceptions;

public class EntityNotFoundException(Type type, int id) : InfrastructureExceptionBase($"Entity of type {type.Name} with id {id} was not found")
{
}
