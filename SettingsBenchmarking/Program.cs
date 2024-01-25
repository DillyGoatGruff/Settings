// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using SettingsBenchmarking;

Console.WriteLine("Hello, World!");

var summary = BenchmarkRunner.Run<Benchmark>();