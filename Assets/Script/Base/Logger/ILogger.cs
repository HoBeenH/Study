using Script.Custom.CustomDebug;

namespace Script.Base.Logger
{
    public interface ILogger<T>
    {
        ILogger<T> Logger { get; }
        void L(object msg) => D.L($"{typeof(T).Name} :: {msg}");
        void W(object msg) => D.W($"{typeof(T).Name} :: {msg}");
        void E(object msg) => D.E($"{typeof(T).Name} :: {msg}");
        void As(bool condition, object msg) => D.As(condition, msg);
    }
}