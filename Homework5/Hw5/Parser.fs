module Hw5.Parser

open System
open System.Globalization
open Hw5.Calculator
open Hw5.MaybeBuilder

let isArgLengthSupported (args:string[]): Result<'a,'b> =
    match args.Length=3 with
    | true -> Ok args 
    | false -> Error Message.WrongArgLength
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | "+" -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | "-" -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | "*" -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | "/" -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | _ -> Error Message.WrongArgFormatOperation

let parseArgs (args: string[]): Result<('a * String * 'b), Message> = //
    let mutable value1 = 0.0
    let mutable value2 = 0.0
    if Double.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture, &value1) &&
       Double.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, &value2) then Ok (value1,args[1],value2)
    else
        Error Message.WrongArgFormat
    
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | CalculatorOperation.Divide ->
        match arg2=0.0 with
        | true -> Error Message.DivideByZero
        | false -> Ok (arg1,operation,arg2)
    | _ -> Ok (arg1,operation,arg2)

let parseCalcArguments (args: string[]): Result<'a, 'b> =
    maybe {
        let! isArgLengthSupported = isArgLengthSupported args
        let! parseArgs = parseArgs isArgLengthSupported
        let! parseOperation = isOperationSupported parseArgs
        let! isDividingByZero = isDividingByZero parseOperation
        return isDividingByZero
    }