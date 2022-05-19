namespace makeITeasy.AppFramework.Models
{
    public abstract class ValidableReadUncommitedQuery<T> : ReadUncommitedQuery<T> where T : IBaseEntity
    {
        public abstract bool IsValid { get; }

        public override void BuildQuery()
        {
            if (!IsValid)
            {
                string errorMessage = $"The query {this.GetType().Name} is not valid, the request was not send to the DB without filters";
                throw new InvalidQueryException(errorMessage);
            }

            DoBuildQuery();
        }

        public abstract void DoBuildQuery();
    }
}
