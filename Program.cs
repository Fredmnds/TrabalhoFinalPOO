using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

public class ConsumidorEncontrado : Exception
{
    public ConsumidorEncontrado()
    {
    }

    public ConsumidorEncontrado(string message) : base(message)
    {
    }

    public ConsumidorEncontrado(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

class Consumidor
{
  private static int ultimoIdConsumidor = 0;
  public int IdConsumidor { get; private set; }
  public string Nome { get; set; }
  public string Cpf { get; set; }
  public List<Conta> Contas { get; set; } = new List<Conta>();

  public Consumidor()
  {
    ultimoIdConsumidor++;
    IdConsumidor = ultimoIdConsumidor;
  }
}

enum TipoConta
{
  Agua = 0,
  Energia = 1,
}

public enum TipoConsumidor
{
  Residencial,
  Comercial
}

class Conta
{
  private static int ultimoIdConta = 0;
  public TipoConta Tipo { get; set; }
  public TipoConsumidor TipoImovel { get; set; }
  public int IdConta { get; private set; }
  public Consumidor Consumidor { get; set; }
  public double LeituraAnterior { get; set; }
  public double LeituraAtual { get; set; }
  public double Consumo { get; set; }
  public double Valor { get; set; }
  public int mes { get; set; }

  public Conta()
  {
    ultimoIdConta++;
    IdConta = ultimoIdConta;
  }
}

class ContaAgua : Conta
{
  public string TipoServico { get; set; }

}

class ContaEnergia : Conta
{
  public string TipoServico { get; set; }
}


class Program
{

  static List<Consumidor> consumidores = new List<Consumidor>();

  static List<string> ArquivosLido = new List<string>();

  static void Sleep(int milissegundos)
  {
    Thread.Sleep(milissegundos);
  }

  static void ErrorMessage(string message)
  {
    Console.Clear();
    Console.WriteLine("----- Erro na execução do programa -----");
    Console.WriteLine(message);
    Console.WriteLine("----------------------------------------");
    Sleep(3000);
    Console.Clear();
  }
  static void ProcessFileContent(StreamReader sr)
  {
    try
    {
      string line;
      Consumidor consumidorAtual = null;
      while ((line = sr.ReadLine()) != null)
      {

        string[] parts = line.Split(';');

        if (parts.Length > 0 && parts[0].Trim().ToLower() == "cliente")
        {
          Consumidor consumidorEncontrado = consumidores.Find(c => c.Cpf == parts[2].Trim());
          if(consumidorEncontrado != null )
            throw new ConsumidorEncontrado("Esse Cpf já se encontra cadastrado!");
          Console.WriteLine(consumidorEncontrado);

          consumidorAtual = new Consumidor
          {
            Nome = parts.Length > 1 ? parts[1].Trim() : string.Empty,
            Cpf = parts.Length > 2 ? parts[2].Trim() : string.Empty,
            Contas = new List<Conta>()
          };

          consumidores.Add(consumidorAtual);

          // Console.WriteLine($"Novo consumidor criado: Nome={consumidorAtual.Nome}, CPF={consumidorAtual.Cpf}");
        }
        else if (parts.Length > 0 && parts[0].Trim().ToLower() == "agua")
        {
          if (consumidorAtual != null)
          {
            double atual = parts.Length > 1 ? double.Parse(parts[1].Trim()) : 0;
            double anterior = parts.Length > 2 ? double.Parse(parts[2].Trim()) : 0;
            double consumo = atual - anterior;

            // Console.WriteLine(line);
            ContaAgua conta1 = new ContaAgua
            {
              Consumidor = consumidorAtual,
              TipoServico = parts.Length > 3 ? parts[3].Trim() : string.Empty,
              LeituraAtual = parts.Length > 1 ? double.Parse(parts[1].Trim()) : 0,
              LeituraAnterior = parts.Length > 2 ? double.Parse(parts[2].Trim()) : 0,
              Tipo = TipoConta.Agua,
              TipoImovel = TipoConsumidor.Comercial,
              Consumo = consumo,
              Valor = 0,
              mes = parts.Length > 4 ? int.Parse(parts[1].Trim()) : 0,
            };

            consumidorAtual.Contas.Add(conta1);


            // Console.WriteLine($"Nova conta de água criada para o consumidor {consumidorAtual.Nome}: LeituraAtual={conta1.LeituraAtual}, LeituraAnterior={conta1.LeituraAnterior}, TipoServico={conta1.TipoServico}");
          }
          else
          {
            Console.WriteLine("Erro: Encontrado registro de água antes de um cliente. Ignorando a conta de água.");
          }
        }
        else if (parts.Length > 0 && parts[0].Trim().ToLower() == "energia")
        {
          if (consumidorAtual != null)
          {
            double atual = parts.Length > 1 ? double.Parse(parts[1].Trim()) : 0;
            double anterior = parts.Length > 2 ? double.Parse(parts[2].Trim()) : 0;
            double consumo = anterior - atual;

            ContaEnergia conta1 = new ContaEnergia
            {
              Consumidor = consumidores.Last(),
              LeituraAtual = atual,
              LeituraAnterior = anterior,
              TipoServico = parts.Length > 3 ? parts[3].Trim() : string.Empty,
              Tipo = TipoConta.Energia,
              Consumo = consumo,
              Valor = 0
            };

            consumidorAtual.Contas.Add(conta1);

            // Console.WriteLine($"Nova conta de Energia criada para o consumidor {consumidores.Last().Nome}: LeituraAtual={conta1.LeituraAtual}, LeituraAnterior={conta1.LeituraAnterior}, TipoServico={conta1.TipoServico}");
          }
          else
          {
            Console.WriteLine("Erro: Encontrado registro de energia antes de um cliente. Ignorando a conta de energia.");
          }
        }
        else
        {
          Console.WriteLine("Linha fora do formato padrão do arquivo, será devidamente ignorada!");
        }
        WriteTheFile();
      }
    }
    catch (FileNotFoundException)
    {
      Console.WriteLine($"O arquivo {sr} não foi encontrado. O programa continuará executando.");
    }
    catch (ConsumidorEncontrado e)
    {
      Console.WriteLine(e.Message);
      // Console.WriteLine("Esse Cpf já se encontra cadastrado.");
      // Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
      Console.WriteLine("Um erro ocorreu durante a leitura do arquivo:");
      // Console.WriteLine(e.Message);
    }
  }


  static double CalcValue(string tipoImovel, string tipo, double consumo)
  {
    try
    {

      if (tipo.ToLower() == "energia")
      {
        if (tipoImovel == "residencial")
        {

          return (0.46 * consumo + 13.25) * 1.4285;
        }
        else
        {
          return (0.41 * consumo + 13.25) * 1.2195;

        }
      }
      else
      {
        return 0;
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }
    return 0;
  }
  static void WriteTheFile()
  {
    try
    {
      StreamWriter sw = new StreamWriter("consumidores.txt");

      foreach (Consumidor consumidor in consumidores)
      {
        sw.WriteLine(consumidor.IdConsumidor + ";" + consumidor.Nome + ";" + consumidor.Cpf);

        foreach (Conta c in consumidor.Contas)
        {

          if (c.Tipo == TipoConta.Agua)
          {
            ContaAgua c2 = (ContaAgua)c;

            sw.WriteLine(c2.Tipo + ";" + c2.LeituraAnterior + ";" + c2.LeituraAtual + ";" + c2.Consumo);
          }
          else if (c.Tipo == TipoConta.Energia)
          {
            ContaEnergia c2 = (ContaEnergia)c;
            sw.WriteLine(c2.Tipo + ";" + c2.LeituraAnterior + ";" + c2.LeituraAtual + ";" + c2.Consumo);
          }
        }
        sw.WriteLine("-");
      }
      sw.Close();
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }
  }

  static void ReadFile()
  {
    Console.Clear();
    Console.WriteLine("Insira o nome do arquivo a ser lido");
    string name = Console.ReadLine();

    try
    {
      using (StreamReader sr = new StreamReader(name + ".txt"))
      {
        ProcessFileContent(sr);
      }
    }
    catch (FileNotFoundException)
    {
      Console.WriteLine($"O arquivo {name} não foi encontrado. O programa continuará executando.");
    }
    catch (Exception e)
    {
      Console.WriteLine($"Um erro ocorreu durante a leitura do arquivo: {e.Message}");
    }
  }

  static void SwitchMenu()
  {
    Console.WriteLine("------ Mini Menu ------");
    Console.WriteLine("0. Sair");
    Console.WriteLine("1. Registro a partir de um arquivo de texto");
    Console.WriteLine("2. Pesquisa específica por consumidor");
    Console.WriteLine("3. Listagem de todos os Consumidores");
    Console.WriteLine("4. Listagem de todos os consumidores e contas");
    Console.WriteLine("5. Opção 5");
    Console.WriteLine("6. Opção 6");
    Console.WriteLine("7. Opção 7");
    Console.WriteLine("8. Opção 8");
    Console.WriteLine("9. Opção 9");

    Console.WriteLine("------------------------");
    Console.WriteLine("Escolha uma opção (0-9): ");
  }

  static void Main()
  {
    int entrada = -1;
    while (entrada != 9)
    {
      SwitchMenu();

      try
      {
        entrada = int.Parse(Console.ReadLine());
      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 0 a 9, de acordo com o menu");
      }
      switch (entrada)
      {
        case 0:
          Console.WriteLine("Finalizando o programa");
          return;
        case 1:
          ReadFile();
          Sleep(2500);
          // Console.Clear();
          break;
        case 2:
          // ReadTheArchive();
          Sleep(2500);
          Console.Clear();
          break;
        case 3:
        Console.Clear();
        if(consumidores.Count > 0){

           Console.WriteLine("------ Listagem de consumidores ------");
          foreach (Consumidor c in consumidores)
          {
            Console.WriteLine("Consumidor " + c.IdConsumidor + ": " + c.Nome);
            Console.WriteLine();
          }
          Console.WriteLine("---------------------------------------");
        }
        else{
          Console.WriteLine("Nenhum consumidor foi adicionado ainda");
        }
          Sleep(5000);
          Console.Clear();
          break;
        case 4:
          Console.WriteLine("------ Listagem de consumidores ------");
          foreach (Consumidor c in consumidores)
          {
            Console.WriteLine("Consumidor " + c.IdConsumidor + ": " + c.Nome);
            Console.WriteLine();
          }
          Sleep(5000);
          Console.Clear();
          break;
        default:
          {
            ErrorMessage("Insira um número válido de 0 a 9, de acordo com o menu");
          }
          break;

      }

    }
    Console.ReadKey();
  }
}