using System;
using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    [SerializeField] private Text _position;
    [SerializeField] private Text _description;
    [SerializeField] private Image _image;

    [SerializeField] private Color[] _teamColors;

    public void Propagate(Warrior elem, Warrior valid, int number, bool debug)
    {
        string letter;
        switch (elem._team)
        {
            case Force.Red:
                _image.color = _teamColors[0];
                letter = "К";
                break;

            case Force.Blue:
                _image.color = _teamColors[1];
                letter = "С";
                break;

            default:
                throw new Exception("bug");
        }

        _position.text = $"# {number + 1}";
        _description.text = $"Существо {letter}{elem._number}:\n" +
                           $"Инициатива - {elem._initiative}, Скорость - {elem._speed}";

        {
            // Показываем, насколько корректно мы отрисовали:
            if (!debug) return;

            _description.text += $", Коэфициент - {elem.Coefficient}";

            var isSame = elem.IsSame(valid);
            _description.text += $" | {isSame}";

            if (!isSame)
            {
                var text = JsonUtility.ToJson(valid);
                _description.text += $" | {text}";
            }
        }
    }
}