using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

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
  public TipoImovel TipoConsumidor { get; set; }
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

public enum TipoImovel
{
  Residencial,
  Comercial
}

class Conta
{
  private static int ultimoIdConta = 0;
  public int IdConta { get; private set; }
  public Consumidor Consumidor { get; set; }
  public TipoConta Tipo { get; set; }
  public double LeituraAnterior { get; set; }
  public double LeituraAtual { get; set; }
  public double Consumo { get; set; }
  public int Mes { get; set; }

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
    Sleep(5000);
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
          if (consumidorEncontrado != null)
            throw new ConsumidorEncontrado("O Cpf: " + parts[2].Trim() + " já se encontra cadastrado!");
          Console.WriteLine(consumidorEncontrado);

          consumidorAtual = new Consumidor
          {
            Nome = parts.Length > 1 ? parts[1].Trim() : string.Empty,
            Cpf = parts.Length > 2 ? parts[2].Trim() : string.Empty,
            TipoConsumidor = parts[3].Trim() == "comercial" ? TipoImovel.Comercial : TipoImovel.Residencial,
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
              Consumo = consumo,
              Mes = parts.Length > 4 ? int.Parse(parts[4].Trim()) : 0,
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

              Mes = parts.Length > 4 ? int.Parse(parts[4].Trim()) : 0,
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


  static void SearchByMonth(Consumidor c)
  {
    int month = -1;
    while (month != 13)
    {
      Console.WriteLine("month" + month);
      Console.WriteLine("Insira o mês atual em número de 2 a 12");
      try
      {
        month = int.Parse(Console.ReadLine());
        if (month < 2 || month > 12)
        {
          Console.WriteLine("Insira um mês válido");
        }
        ContaAgua contaAgua = c.Contas.OfType<ContaAgua>().FirstOrDefault(c => c.Mes == month - 1);
        ContaEnergia contaEnergia = c.Contas.OfType<ContaEnergia>().FirstOrDefault(c => c.Mes == month - 1);
        Console.WriteLine("Consumo da conta de Água do Mês passado: " + contaAgua.Consumo);
        Console.WriteLine("Consumo da conta de Energia do Mês passado: " + contaAgua.Consumo);
        Sleep(7000);
        month = 13;
        Console.Clear();

      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 2 a 12, de acordo com os meses.");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

  }
  static void Search(string cpf)
  {
    Console.Clear();
    Consumidor consumidorEncontrado = consumidores.Find(c => c.Cpf == cpf);
    if (consumidorEncontrado == null)
    {
      Console.WriteLine("Nenhum consumidor com o cpf " + cpf + " foi encontrado!");
    }
    else
    {

      int entrada = -1;

      while (entrada != 6)
      {
        Console.WriteLine("------ Consumidor com o cpf buscado: " + consumidorEncontrado.Nome + " ------");
        Console.WriteLine("0. Consumo de energia/água no último mês");
        Console.WriteLine("1. Valor total da conta");
        Console.WriteLine("2. Valor da conta sem impostos");
        Console.WriteLine("3. Variação de minha conta em reais e em consumo, entre dois meses escolhidos?");
        Console.WriteLine("4. Valor médio de minha conta de energia/água?");
        Console.WriteLine("5. Mês que houve a conta de maior valor, em reais e em consumo?");
        Console.WriteLine("6. Voltar");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("Escolha uma opção (0-6): ");
        try
        {
          entrada = int.Parse(Console.ReadLine());
        }
        catch (FormatException e)
        {
          ErrorMessage("Insira um número válido de 0 a 6, de acordo com o menu");
        }
        switch (entrada)
        {
          case 0:
            SearchByMonth(consumidorEncontrado);
            break;
          case 6:
            return;
            break;
          default:
            {
              ErrorMessage("Insira um número válido de 0 a 6, de acordo com o menu");
            }
            break;
        }
      }
    }

  }
  // static double CalcValue(string tipoImovel, string tipo, double consumo)
  // {
  //   try
  //   {

  //     if (tipo.ToLower() == "energia")
  //     {
  //       if (tipoImovel == "residencial")
  //       {

  //         return (0.46 * consumo + 13.25) * 1.4285;
  //       }
  //       else
  //       {
  //         return (0.41 * consumo + 13.25) * 1.2195;

  //       }
  //     }
  //     else
  //     {
  //       return 0;
  //     }
  //   }
  //   catch (Exception e)
  //   {
  //     Console.WriteLine(e.Message);
  //   }
  //   return 0;
  // }

  static void WriteTheFile()
  {
    try
    {
      StreamWriter sw = new StreamWriter("consumidores.txt");

      foreach (Consumidor consumidor in consumidores)
      {
        sw.WriteLine(consumidor.IdConsumidor + "; " + consumidor.Nome + "; " + consumidor.Cpf + "; " + consumidor.TipoConsumidor) ;

        foreach (Conta c in consumidor.Contas)
        {

          if (c.Tipo == TipoConta.Agua)
          {
            ContaAgua c2 = (ContaAgua)c;

            sw.WriteLine(c2.Tipo + "; " + c2.LeituraAnterior + "; " + c2.LeituraAtual + "; " + c2.Consumo);
          }
          else if (c.Tipo == TipoConta.Energia)
          {
            ContaEnergia c2 = (ContaEnergia)c;
            sw.WriteLine(c2.Tipo + "; " + c2.LeituraAnterior + "; " + c2.LeituraAtual + "; " + c2.Consumo);
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
    Console.Clear();
    Console.WriteLine("------ Mini Menu ------");
    Console.WriteLine("0. Registro a partir de um arquivo de texto");
    Console.WriteLine("1. Pesquisa específica por consumidor");
    Console.WriteLine("2. Listagem de todos os Consumidores");
    Console.WriteLine("3. Listagem de todos os consumidores e contas");
    Console.WriteLine("4. Finalizar o programa");
    Console.WriteLine("------------------------");
    Console.WriteLine("Escolha uma opção (0-4): ");
  }

  static void Main()
  {
    int entrada = -1;
    while (entrada != 4)
    {
      Console.Clear();
      SwitchMenu();

      try
      {
        entrada = int.Parse(Console.ReadLine());
      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 0 a 4, de acordo com o menu");
      }
      switch (entrada)
      {
        case 0:
          ReadFile();
          Sleep(2500);
          // Console.Clear();
          break;
        case 1:
          Console.WriteLine("Insira um Cpf para buscar entre os consumidores");
          string cpf = Console.ReadLine();
          Search(cpf);
          Sleep(2500);
          Console.Clear();
          break;

        case 2:
          Console.Clear();
          if (consumidores.Count > 0)
          {

            Console.WriteLine("------ Listagem de consumidores ------");
            foreach (Consumidor c in consumidores)
            {
              Console.WriteLine("Consumidor " + c.IdConsumidor + ": " + c.Nome);
              Console.WriteLine();
            }
            Console.WriteLine("---------------------------------------");
          }
          else
          {
            Console.WriteLine("Nenhum consumidor foi adicionado até o momento. Favor inserir através de um arquivo txt.");
          }
          Sleep(5000);
          Console.Clear();
          break;

        case 3:
          Console.Clear();
          if (consumidores.Count > 0)
          {
            Console.WriteLine("------ Listagem de consumidores e suas contas ------");
            foreach (Consumidor c in consumidores)
            {
              Console.WriteLine("Consumidor " + c.IdConsumidor + ": " + c.Nome);
              foreach (Conta conta in c.Contas)
              {
                Console.WriteLine("Leitura atual: " + conta.LeituraAtual + "; Consumo: " + conta.Consumo);
              }
              Console.WriteLine();
            }
            Console.WriteLine("---------------------------------------");
          }
          else
          {
            Console.WriteLine("Nenhum consumidor foi adicionado até o momento. Favor inserir através de um arquivo txt.");
          }
          Sleep(5000);
          Console.Clear();
          break;
        case 4:
          Console.WriteLine("Finalizando o programa");
          return;

        default:
          {
            ErrorMessage("Insira um número válido de 0 a 4, de acordo com o menu");
          }
          break;

      }

    }
    Console.ReadKey();
  }
}