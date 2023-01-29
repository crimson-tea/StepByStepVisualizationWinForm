# StepByStepVisualizationWinForm

### RedoUndoを利用してアルゴリズムの各段階を可視化しました

1. RedoUndoのデモ
2. BinarySearch
3. 素数判別
* エラトステネスの篩
* アトキンの篩
4. 迷路探索
* 深さ優先探索（より良い順番での探索）
* 深さ優先探索（より悪い順番での探索）
* 幅優先探索
* ダイクストラ法
* A*
* A*（完璧な推定値）

### 使用言語、フレームワーク
* C# + WinForms

### 必要条件
* Windows10 以上
* .NET 6.0 以上

### 使い方
#### Visual Studio
このリポジトリのコードをDownloadZipでダウンロードします。

ダウンロードしたファイルを展開し、`StepByStepVisualizationWinForm.sln`ファイルをVisualStudioで開いてください。

#### Visual Studio Code
ターミナルより、以下のコマンドで起動できます。
```
clone https://github.com/crimson-tea/StepByStepVisualizationWinForm.git
cd StepByStepVisualizationWinForm/StepByStepVisualizationWinForm/
dotnet run
```
dotnetコマンドが利用できない方は[こちらのインストール方法](https://learn.microsoft.com/ja-jp/dotnet/core/install/windows?tabs=net70)をご確認ください。

### こだわりポイント
RedoUndoの実装方法を工夫して、少しのコードでRedoUndoが利用できるようにしています。


### 今後の計画
WPFやblazorでも同様のことができないか試したいと考えています。
