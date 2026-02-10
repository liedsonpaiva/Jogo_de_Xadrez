using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // Referência definida pela Unity (Prefab da peça de xadrez)
    public GameObject chesspiece;

    public GameObject white_queen, white_rook, white_bishop, white_knight;
    public GameObject black_queen, black_rook, black_bishop, black_knight;

    [HideInInspector] public bool waitingPromotion = false;
    [HideInInspector] public string promotionPlayer;
    [HideInInspector] public int promotionX;
    [HideInInspector] public int promotionY;

    public GameObject promotionUI;

    // Matrizes necessárias para armazenar as posições das peças no tabuleiro
    // Também existem arrays separados para cada jogador, facilitando o controle
    // Os mesmos objetos existem tanto em "positions" quanto em "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    // Jogador atual ("white" ou "black")
    private string currentPlayer = "white";

    // Controle de fim de jogo
    private bool gameOver = false;

    // Método chamado automaticamente pela Unity quando o jogo inicia
    public void Start()
    {
        playerWhite = new GameObject[] { Create("white_rook", 0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0), Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1) };
        playerBlack = new GameObject[] { Create("black_rook", 0, 7), Create("black_knight",1,7),
            Create("black_bishop",2,7), Create("black_queen",3,7), Create("black_king",4,7),
            Create("black_bishop",5,7), Create("black_knight",6,7), Create("black_rook",7,7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6) };

        // Define todas as posições iniciais no tabuleiro lógico
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); // Acessa o script Chessman da peça
        cm.name = name; // Define o nome da peça (usado para identificar tipo e cor)
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); // Ativa a peça (define sprite, jogador, posição visual etc.)
        return obj;
    }

    public void StartPromotion(string player, int x, int y)
    {
        waitingPromotion = true;
        promotionPlayer = player;
        promotionX = x;
        promotionY = y;

        promotionUI.SetActive(true);
    }

    public void Promote(string pieceName)
    {
        // Remove o peão da matriz
        SetPositionEmpty(promotionX, promotionY);

        // Cria nova peça usando o MESMO fluxo do jogo
        string fullName = promotionPlayer + "_" + pieceName;

        GameObject obj = Instantiate(chesspiece, Vector3.zero, Quaternion.identity);

        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = fullName;
        cm.SetXBoard(promotionX);
        cm.SetYBoard(promotionY);
        cm.hasMoved = true;
        cm.Activate(); // ← ISSO É O PONTO CRÍTICO

        SetPosition(obj);

        promotionUI.SetActive(false);
        waitingPromotion = false;

        NextTurn();
    }
    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        // Sobrescreve qualquer peça que estivesse naquela posição
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            
            SceneManager.LoadScene("Game"); // Recarrega a cena do jogo
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;

        // Exibe o texto do vencedor
        string vencedor = playerWinner == "white" ? "Brancas" : "Pretas";

        Text winnerText = GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>();
        winnerText.enabled = true;
        winnerText.text = vencedor + " venceram!";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}