using WebApplication1.Models;

namespace WebApplication1;

public class GameSession
{
    private Lobby lobby;
    
    
    async void Start()
    {
        
        Update();
    }

    async void Update()
    {
        while (true)
        {
            
            
            
            
        }
    }

    public void SetLobby(Lobby _lobby)
    {
        lobby = _lobby;
    }
    
}