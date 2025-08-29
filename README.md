# C# (.NET 9 NativeAOT) によるネイティブDLL作成サンプル

.NET 9 の NativeAOT 機能を利用して、C#でネイティブDLLを作成するサンプルプロジェクトです。
このサンプルは、C言語の呼び出し規約(`cdecl`)に準拠しており、秀丸エディタのマクロなど、ネイティブDLLを呼び出すことができる様々なアプリケーションから利用することを想定しています。

![.NET 9.0 NativeAOT](https://img.shields.io/badge/.NET_v9.0-NativeAOT-6479ff.svg)
![x64 | x86](https://img.shields.io/badge/x64_|_x86-both-6479ff.svg)

## 概要 (Overview)

このプロジェクトは、マネージドコードであるC#を、アンマネージドなネイティブコードにコンパイル(Ahead-Of-Time)し、単一のDLLファイルとして出力します。
これにより、.NETランタイムがインストールされていない環境でも動作する、自己完結型で高速なネイティブライブラリを作成できます。

## 機能 (Features)

- **.NET 9 NativeAOT**: 最新の.NET AOTコンパイラ技術を利用。
- **Cdecl呼び出し規約**: C/C++や多くのネイティブアプリケーションと互換性のある関数をエクスポート。
- **x64 & x86対応**: 64bitおよび32bitの両方のアーキテクチャ向けにビルド可能。
- **自己完結型**: .NETランタイムのインストールが不要な単一DLL。

## エクスポートされる関数 (Exported Functions)

このDLLは以下の2つの関数をエクスポートします。

### 1. `doubleint`

整数値を2倍にして返します。

- **シグネチャ:** `nint doubleint(nint value)`
- **引数:**
  - `value` (nint): 2倍にする整数。
- **戻り値:**
  - `value` を2倍にした整数。

### 2. `sumstring`

2つのUnicode文字列を連結して、新しい文字列へのポインタを返します。

- **シグネチャ:** `IntPtr sumstring(IntPtr str1, IntPtr str2)`
- **引数:**
  - `str1` (IntPtr): 1つ目の文字列へのポインタ (UTF-16/Unicode)。
  - `str2` (IntPtr): 2つ目の文字列へのポインタ (UTF-16/Unicode)。
- **戻り値:**
  - 連結された新しい文字列へのポインタ。

**注意:** この関数が返すポインタは、次に`sumstring`関数が呼び出されるまで有効です。内部的に静的な領域を再利用しているため、返されたポインタを長期間保持しないでください。

## ビルド方法 (How to Build)

ビルドには [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) が必要です。

1. コマンドプロンプトやターミナルを開き、プロジェクトのルートディレクトリに移動します。
2. 以下のコマンドを実行してビルドします。

**x64 (64bit) 版をビルドする場合:**
```bash
dotnet publish -r win-x64 -c Release
```

**x86 (32bit) 版をビルドする場合:**
```bash
dotnet publish -r win-x86 -c Release
```

ビルドが成功すると、以下のパスにDLLが出力されます。
`ClassLibrary8/ClassLibrary8/bin/Release/net9.0/[RID]/publish/ClassLibrary8.dll`

`[RID]` は `win-x64` や `win-x86` に対応します。

## 使用方法 (How to Use)

C/C++などのネイティブ言語からDLLをロードし、`GetProcAddress`等で関数ポインタを取得して呼び出します。

以下はC++での呼び出し例です。

```cpp
#include <iostream>
#include <windows.h>

// 関数の型を定義
typedef intptr_t (*DoubleIntFunc)(intptr_t);
typedef const wchar_t* (*SumStringFunc)(const wchar_t*, const wchar_t*);

int main() {
    // DLLをロード
    HMODULE hDll = LoadLibrary(L"ClassLibrary8.dll");
    if (!hDll) {
        std::cerr << "DLLのロードに失敗しました。" << std::endl;
        return 1;
    }

    // doubleint関数を取得
    DoubleIntFunc doubleint = (DoubleIntFunc)GetProcAddress(hDll, "doubleint");
    if (doubleint) {
        intptr_t result = doubleint(10);
        std::cout << "doubleint(10) = " << result << std::endl; // 20が出力される
    }

    // sumstring関数を取得
    SumStringFunc sumstring = (SumStringFunc)GetProcAddress(hDll, "sumstring");
    if (sumstring) {
        const wchar_t* combined = sumstring(L"Hello, ", L"World!");
        std::wcout << L"sumstring(\"Hello, \", \"World!\") = " << combined << std::endl; // "Hello, World!"が出力される
    }

    // DLLをアンロード
    FreeLibrary(hDll);

    return 0;
}
```

秀丸エディタのマクロからは、`loaddll`文でDLLをロードし、`dllfunc`や`dllfuncw`で関数を呼び出すことができます。
