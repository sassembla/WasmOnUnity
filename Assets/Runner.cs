using System.Collections.Generic;
using UnityEngine;
using Wasm;
using Wasm.Interpret;

public class Runner : MonoBehaviour
{
    // wasmへと値を渡す関数を定義
    private IReadOnlyList<object> GetParam(IReadOnlyList<object> args)
    {
        Debug.Log("呼ばれてる、、？");
        return new object[] { 100, };
    }

    // Start is called before the first frame update
    void Start()
    {
        // wasmファイルを読み込む
        string path = Application.streamingAssetsPath + "/something.wasm";
        WasmFile file = WasmFile.ReadBinary(path);

        // importerを生成
        var importer = new PredefinedImporter();

        // 関数定義情報の注入
        importer.DefineFunction(
            "GetParam",// 関数名
            new DelegateFunctionDefinition(// 関数の定義
                new WasmValueType[] { },
                new[] { WasmValueType.Int32, },
                GetParam
            )
        );

        // wasmをインスタンス化
        ModuleInstance module = ModuleInstance.Instantiate(file, importer);

        // インスタンスから、定義済み関数の取得を試みる
        if (module.ExportedFunctions.TryGetValue("Something", out FunctionDefinition funcDef))
        {
            // 関数が見つかったらそれを実行
            IReadOnlyList<object> results = funcDef.Invoke(new object[] { 1, });
            Debug.Log("定義があったよ＝＝〜" + results[0]);
        }
    }
}