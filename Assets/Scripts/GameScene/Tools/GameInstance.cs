using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class GameInstance
{
    private static GameInstance instance;
    public static GameInstance Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameInstance();
            }
            return instance;
        }
    }
    
    private PlayerController playerController;
    public static void SetPlayerController(PlayerController playerController)
    {
        Instance.playerController = playerController;
    }

    public static PlayerController GetPlayerController()
    {
        return Instance.playerController;
    }

}

