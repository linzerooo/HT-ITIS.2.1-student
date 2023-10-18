module Hw6.Hw6.Client.Program
open System
open System.Net.Http


let sendRequestAsync (client: HttpClient) (url: string) =
    async {
        let! response = Async.AwaitTask (client.GetAsync url)
        let! result = Async.AwaitTask (response.Content.ReadAsStringAsync())
        return result
    }
    
[<EntryPoint>]
let main args =
    use handler = new HttpClientHandler()
    use client = new HttpClient(handler)
    let input = Console.ReadLine()
    let args = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
    if args.Length = 3 then
        let url = $"http://localhost:58592/calculate?value1={args[0]}&operation={parseOperation args[1]}&value2={args[2]}";
        printfn $"{Async.RunSynchronously(sendRequestAsync client url)}"
    0