using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    // Algumas funções precisarão de referência ao controlador do jogo
    public GameObject controller;

    // A peça de xadrez que foi clicada para criar este MovePlate
    GameObject reference = null;

    // Localização no tabuleiro (matriz)
    int matrixX;
    int matrixY;

    // false: movimento normal | true: ataque
    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            // Define a cor vermelha para indicar ataque
            gameObject.GetComponent<SpriteRenderer>().color =
                new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Chessman chessman = reference.GetComponent<Chessman>();

        // ATAQUE
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>()
                                      .GetPosition(matrixX, matrixY);

            if (cp.name == "white_king")
                controller.GetComponent<Game>().Winner("black");

            if (cp.name == "black_king")
                controller.GetComponent<Game>().Winner("white");

            Destroy(cp);
        }

        int startX = chessman.GetXBoard();
        int startY = chessman.GetYBoard();

        // LIMPA A POSIÇÃO ANTIGA NO TABULEIRO
        controller.GetComponent<Game>()
                  .SetPositionEmpty(startX, startY);

        // MOVE A PEÇA PARA A NOVA POSIÇÃO
        chessman.SetXBoard(matrixX);
        chessman.SetYBoard(matrixY);
        chessman.SetCoords();
        chessman.hasMoved = true;

        // === ROQUE ===
        bool isKing = chessman.name.Contains("king");
        bool isCastle = Mathf.Abs(matrixX - startX) == 2;

        if (isKing && isCastle)
        {
            // ROQUE CURTO
            if (matrixX == 6)
            {
                GameObject rook = controller.GetComponent<Game>()
                                            .GetPosition(7, startY);
                Chessman rookMan = rook.GetComponent<Chessman>();

                controller.GetComponent<Game>()
                          .SetPositionEmpty(7, startY);

                rookMan.SetXBoard(5);
                rookMan.SetYBoard(startY);
                rookMan.SetCoords();
                rookMan.hasMoved = true;

                controller.GetComponent<Game>().SetPosition(rook);
            }

            // ROQUE GRANDE
            if (matrixX == 2)
            {
                GameObject rook = controller.GetComponent<Game>()
                                            .GetPosition(0, startY);
                Chessman rookMan = rook.GetComponent<Chessman>();

                controller.GetComponent<Game>()
                          .SetPositionEmpty(0, startY);

                rookMan.SetXBoard(3);
                rookMan.SetYBoard(startY);
                rookMan.SetCoords();
                rookMan.hasMoved = true;

                controller.GetComponent<Game>().SetPosition(rook);
            }
        }

        // ATUALIZA A MATRIZ COM A NOVA POSIÇÃO DA PEÇA
        controller.GetComponent<Game>().SetPosition(reference);

        // PASSA PARA O PRÓXIMO TURNO
        controller.GetComponent<Game>().NextTurn();

        // REMOVE TODOS OS MOVEPLATES
        chessman.DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
