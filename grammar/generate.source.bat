echo off
echo '--------------------------------------------'
echo ' source generation C# - Riql                '
echo '--------------------------------------------'
echo on
"%JAVA_HOME%\bin\java.exe" -jar antlr-4.10.1-complete.jar Riql.g4 -o ../src/Riql/Transpiler/generated -Dlanguage=CSharp -no-listener -visitor -encoding UTF-8 -package Riql.Transpiler
"%JAVA_HOME%\bin\java.exe" -jar antlr-4.10.1-complete.jar Rsql.g4 -o ../src/Riql/Transpiler/Rsql/generated -Dlanguage=CSharp -no-listener -visitor -encoding UTF-8 -package Riql.Transpiler.Rsql
