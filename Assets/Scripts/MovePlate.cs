using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    // Referência ao controlador do jogo
    public GameObject controller;

    // Peça de xadrez que originou este MovePlate
    GameObject reference = null;

    // Posição no tabuleiro (matriz)
    int matrixX;
    int matrixY;

    // false: movimento normal e true: ataque
    public bool attack = false;

    void Start()
    {
        if (attack)
        {
            // Cor vermelha para indicar ataque
            gameObject.GetComponent<SpriteRenderer>().color =
                new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (controller.GetComponent<Game>().waitingPromotion)
            return;

        Chessman chessman = reference.GetComponent<Chessman>();

        // === ATAQUE ===
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

        // Limpa a posição antiga
        controller.GetComponent<Game>()
                  .SetPositionEmpty(startX, startY);

        // Move a peça para a nova posição
        chessman.SetXBoard(matrixX);
        chessman.SetYBoard(matrixY);
        chessman.SetCoords();
        chessman.hasMoved = true;

        // === ROQUE ===
        bool isKing = chessman.name.Contains("king");
        bool isCastle = Mathf.Abs(matrixX - startX) == 2;

        if (isKing && isCastle)
        {
            // Roque curto
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

            // Roque grande
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

        // === PROMOÇÃO DO PEÃO (SUBSTITUIÇÃO) ===
        bool isPawn = chessman.name.Contains("pawn");
        bool promoteWhite = isPawn && chessman.player == "white" && matrixY == 7;
        bool promoteBlack = isPawn && chessman.player == "black" && matrixY == 0;

        if (promoteWhite || promoteBlack)
        {
            Destroy(reference);
            controller.GetComponent<Game>()
                      .StartPromotion(chessman.player, matrixX, matrixY);
            chessman.DestroyMovePlates();
            return;
        }

        // Atualiza a matriz com a nova posição
        controller.GetComponent<Game>().SetPosition(reference);

        // Próximo turno
        controller.GetComponent<Game>().NextTurn();

        // Remove todos os MovePlates
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
