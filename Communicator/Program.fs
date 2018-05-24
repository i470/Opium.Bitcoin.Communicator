﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

module Opium.Communicator
open System
open System.Diagnostics
  

let runProc filename args startDir : seq<string> * seq<string> = 
    let timer =System.Diagnostics.Stopwatch.StartNew()
    let procStartInfo = 
        ProcessStartInfo(
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            FileName = filename,
            Arguments = args
        )
    match startDir with | Some d -> procStartInfo.WorkingDirectory <- d | _ -> ()

    let outputs = System.Collections.Generic.List<string>()
    let errors = System.Collections.Generic.List<string>()
    let outputHandler f (_sender:obj) (args:DataReceivedEventArgs) = f args.Data
    use p = new Process(StartInfo = procStartInfo)
    p.OutputDataReceived.AddHandler(DataReceivedEventHandler (outputHandler outputs.Add))
    p.ErrorDataReceived.AddHandler(DataReceivedEventHandler (outputHandler errors.Add))
    let started = 
        try
            p.Start()
        with | ex ->
            ex.Data.Add("filename", filename)
            reraise()
    if not started then
        failwithf "Failed to start process %s" filename
    printfn "Started %s with pid %i" p.ProcessName p.Id
    p.BeginOutputReadLine()
    p.BeginErrorReadLine()
    p.WaitForExit()
    timer.Stop()
    outputs.Add(timer.ElapsedMilliseconds.ToString())
    let cleanOut l = l |> Seq.filter (fun o -> String.IsNullOrEmpty o |> not)
    cleanOut outputs,cleanOut errors