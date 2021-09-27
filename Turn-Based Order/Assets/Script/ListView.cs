using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListView : MonoBehaviour
{
    [SerializeField] private TextAsset _dataPrefab;
    [SerializeField] private GameObject _roundPrefab;
    [SerializeField] private GameObject _elementPrefab;

    [SerializeField] private Button _next;
    [SerializeField] private Button _prev;
    [SerializeField] private Button _kick;
    [SerializeField] private GameObject _view;
    [SerializeField] private Toggle _needDebug;

    private int _offset;

    private List<Warrior> _items;
    private Dictionary<int, List<Warrior>> _test;

    private const int Count = 20;

    private int ItemsInRound => _items.Distinct().Count();

    private List<Warrior> Calculate()
    {
        var offset = _offset % _items.Count;
        var round = (_offset / _items.Count) + 1;

        _items.Sort(new WarriorComparer(round));
        var items = _items.GetRange(offset, _items.Count - offset);


        for (var current = round + 1; items.Count < Count; current++)
        {
            _items.Sort(new WarriorComparer(current));

            var count = Count - items.Count;
            if (count > _items.Count)
            {
                count = _items.Count;
            }

            var append = _items.GetRange(0, count);

            for (var j = 0; j < append.Count && items.Count < Count; j++)
            {
                items.Add(append[j % append.Count]);
            }
        }

        return items;
    }

    private void Render()
    {
        foreach (Transform child in _view.transform)
        {
            Destroy(child.gameObject);
        }

        var items = Calculate();
        var show = _offset % ItemsInRound < 9;
        for (var i = 0; i < Count; ++i)
        {
            var round = (i + _offset) / ItemsInRound + 1;

            var index = i + _offset;

            if (show || index % _items.Count == 0)
            {
                show = false;

                var obj = Instantiate(_roundPrefab, _view.transform);
                obj.GetComponentInChildren<Text>().text = $"Раунд {round}";
            }

            var elem = Instantiate(_elementPrefab, _view.transform);
            var valid = _test[round % 2][index % _items.Count];
            elem.GetComponent<Element>().Propagate(items[i], valid, i + _offset, _needDebug.isOn);
        }
    }

    private void OnNext()
    {
        _offset++;

        if (_offset > 0)
        {
            _prev.enabled = true;
            _prev.interactable = true;
        }

        Render();
    }

    private void OnPrev()
    {
        _offset--;

        if (_offset <= 0)
        {
            _prev.enabled = false;
            _prev.interactable = false;
        }

        Render();
    }

    private void OnKick()
    {
        if (_items.Count <= 1)
        {
            return;
        }

        var items = Calculate();
        var drops = items.ElementAt(1);

        // удаляем из списка
        _items = _items.Where(item => !item.IsSame(drops)).ToList();

        // удаляем из тестовых данных
        _test[0] = _test[0].Where(item => !item.IsSame(drops)).ToList();
        _test[1] = _test[1].Where(item => !item.IsSame(drops)).ToList();

        Render();
    }

    private void OnChangeDebug(bool _)
    {
        Render();
    }

    public void Awake()
    {
        _next.onClick.AddListener(OnNext);
        _prev.onClick.AddListener(OnPrev);
        _kick.onClick.AddListener(OnKick);

        _needDebug.onValueChanged.AddListener(OnChangeDebug);

        _prev.enabled = false;
        _prev.interactable = false;

        {
            // Загружаем данные из JSON
            var data = JsonUtility.FromJson<WarriorData>(_dataPrefab.text);

            // воины:
            _items = data._warriors;

            // данные для проверки:
            _test = new Dictionary<int, List<Warrior>>()
            {
                {0, data._second},
                {1, data._first},
            };
        }

        Render();
    }
}