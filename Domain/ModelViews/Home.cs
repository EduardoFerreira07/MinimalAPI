namespace MinimalAPI.Domain.ModelViews;

public struct Home
{
    public string Doc { get => "/swagger"; }

    public string Mensagem { get => "Bem vindo a API! - MinimalAPI"; }

}