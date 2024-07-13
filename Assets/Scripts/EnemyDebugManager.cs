using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDebugManager : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Button _button;
    [SerializeField] private Dropdown _options_FuncNames;
    [SerializeField] private InputField[] _inputParams;

    [SerializeField] private Collider _collider;

    /// <summary>
    /// 현재 클래스에서 메서드 이름 받아서
    /// 라디오버튼에 넣어주고
    /// 그 메서드 이름이 정해졌을때
    /// 매개변수 수만큼 인풋필드 켜주고 실행시 그 수만큼 string을 보냄
    /// </summary>

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);

        string[] reflectionMethods = GetReflectionMethodNames();

        foreach (var methodName in reflectionMethods)
        {
            _options_FuncNames.options.Add(new Dropdown.OptionData(methodName));
        }

        _options_FuncNames.onValueChanged.AddListener(OnOptionSet);
        _options_FuncNames.onValueChanged.Invoke(0);
    }

    private string[] GetReflectionMethodNames()
    {

        var methods = this.GetType().GetMethods();
        return methods
            .Where(method => IsReflection(method.Name))
            .Select(method => method.Name)
            .ToArray();
    }

    private bool IsReflection(string methodName)
    {
        return methodName.Contains("Reflection") ? true : false;
    }
    private void OnButtonClick()
    {
        string methodName = _options_FuncNames.options[_options_FuncNames.value].text;
        ParameterInfo[] paramNames = this.GetType().GetMethod(methodName).GetParameters();
        CallFunc(methodName, paramNames.Length, _inputParams);
    }

    private void OnOptionSet(int index)
    {
        if(_options_FuncNames.options.Count == 0)
        {
            return;
        }

        string methodName = _options_FuncNames.options[_options_FuncNames.value].text;
        ParameterInfo[] paramNames = this.GetType().GetMethod(methodName).GetParameters();
        for (int i = 0; i < _inputParams.Length ; i++)
        {
            if(i < paramNames.Length)
            {
                _inputParams[i].placeholder.GetComponent<Text>().text = paramNames[i].Name;
                _inputParams[i].gameObject.SetActive(false);
                _inputParams[i].gameObject.SetActive(true);
            }
            else
            {
                _inputParams[i].gameObject.SetActive(false);
            }
        }
    }

    private void CallFunc(string methodName, int paramCount, InputField[] inputParams)
    {
        string[] stringparams = new string[paramCount];
        for (int i = 0; i < stringparams.Length; i++)
        {
            stringparams[i] = inputParams[i].text;
        }

        this.GetType().GetMethod(methodName)
            ?.Invoke(this, stringparams);
    }




    public void TestDamageCollider_Reflection()
    {
        StartCoroutine(OnOff(_collider, .1f));
    }

    private IEnumerator OnOff(Component componentm, float time)
    {
        _collider.enabled = true;
        yield return new WaitForSeconds(time);
        _collider.enabled = false;
    }



}
