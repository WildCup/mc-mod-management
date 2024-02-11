namespace McHelper.Infrastructure.Exceptions;

public class EntityAlreadyExistsException(Type type, int id) : InfrastructureExceptionBase($"Entity of type {type.Name} with id {id} already exists")
{
}
