namespace AmberTower.Client.Infrastructure.Auth
{
    public interface IAuthSessionStore
    {
        AuthSession Load();

        void Save(AuthSession session);

        void Clear();
    }
}
