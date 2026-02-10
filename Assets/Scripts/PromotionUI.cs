using UnityEngine;

public class PromotionUI : MonoBehaviour
{
    public GameObject controller;

    public void ChooseQueen()
    {
        controller.GetComponent<Game>().Promote("queen");
    }

    public void ChooseRook()
    {
        controller.GetComponent<Game>().Promote("rook");
    }

    public void ChooseBishop()
    {
        controller.GetComponent<Game>().Promote("bishop");
    }

    public void ChooseKnight()
    {
        controller.GetComponent<Game>().Promote("knight");
    }
}
