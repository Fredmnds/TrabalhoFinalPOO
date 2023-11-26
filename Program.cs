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

          consumidorAtual = new Consumidor
          {
            Nome = parts.Length > 1 ? parts[1].Trim() : string.Empty,
            Cpf = parts.Length > 2 ? parts[2].Trim() : string.Empty,
            TipoConsumidor = parts[3].Trim() == "comercial" ? TipoImovel.Comercial : TipoImovel.Residencial,
            Contas = new List<Conta>()
          };

          consumidores.Add(consumidorAtual);

        }
        else if (parts.Length > 0 && parts[0].Trim().ToLower() == "agua")
        {
          if (consumidorAtual != null)
          {
            double anterior = parts.Length > 1 ? double.Parse(parts[1].Trim()) : 0;
            double atual = parts.Length > 2 ? double.Parse(parts[2].Trim()) : 0;
            double consumo = atual - anterior;

            // Console.WriteLine(line);
            ContaAgua conta1 = new()
            {
              Consumidor = consumidorAtual,
              LeituraAnterior = anterior,
              LeituraAtual = atual,
              Tipo = TipoConta.Agua,
              Consumo = consumo,
              Mes = parts.Length > 3 ? int.Parse(parts[3].Trim()) : 0,
            };

            consumidorAtual.Contas.Add(conta1);
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
            double anterior = parts.Length > 1 ? double.Parse(parts[1].Trim()) : 0;
            double atual = parts.Length > 2 ? double.Parse(parts[2].Trim()) : 0;
            double consumo = atual - anterior;

            ContaEnergia conta1 = new()
            {
              Consumidor = consumidores.Last(),
              LeituraAtual = atual,
              LeituraAnterior = anterior,
              Tipo = TipoConta.Energia,
              Consumo = consumo,
              Mes = parts.Length > 3 ? int.Parse(parts[3].Trim()) : 0,
            };
            consumidorAtual.Contas.Add(conta1);
          }
        }
      }
      Console.Clear();
      Console.WriteLine("Arquivo foi lido com sucesso!");
      WriteTheFile();
    }
    catch (FileNotFoundException)
    {
      Console.WriteLine($"O arquivo {sr} não foi encontrado. O programa continuará executando.");
    }
    catch (ConsumidorEncontrado e)
    {
      Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
      Console.WriteLine("Um erro ocorreu durante a leitura do arquivo:");
    }
  }
  static void SearchByMonth(Consumidor c)
  {
    int month = -1;
    while (month != 0)
    {
      Console.WriteLine("Insira o mês atual em número de 1 a 12 (Insira 0 caso queira retornar para o menu.)");
      try
      {
        month = int.Parse(Console.ReadLine());
        if (month < 1 || month > 12)
        {
          Console.WriteLine("Insira um mês válido");
        }
        ContaAgua contaAgua = c.Contas.OfType<ContaAgua>().FirstOrDefault(c => c.Mes == month - 1);
        ContaEnergia contaEnergia = c.Contas.OfType<ContaEnergia>().FirstOrDefault(c => c.Mes == month - 1);
        Console.Clear();
        Console.WriteLine("---------------------------------------");
        Console.WriteLine("Consumo da conta de Água do Mês passado: " + contaAgua.Consumo);
        Console.WriteLine("Consumo da conta de Energia do Mês passado: " + contaEnergia.Consumo);
        Console.WriteLine("---------------------------------------");
        Sleep(7000);
        month = 0;
        Console.Clear();

      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 2 a 12, de acordo com os meses.");
      }
      catch (Exception e)
      {
        Console.WriteLine("Retornando para o menu...");
      }
    }

  }


  static void CalcValueWithTax(Consumidor c)
  {
    int month = -1;
    while (month != 13)
    {
      try
      {
        Console.WriteLine("Insira o mês para realizar a conta pela conta em número de 1 a 12");
        month = int.Parse(Console.ReadLine());

        if (month < 1 || month > 12)
        {
          Console.WriteLine("Insira um mês válido");
        }

        ContaAgua contaAgua = c.Contas.OfType<ContaAgua>().FirstOrDefault(c => c.Mes == month);
        ContaEnergia contaEnergia = c.Contas.OfType<ContaEnergia>().FirstOrDefault(c => c.Mes == month);

        double valorAgua = contaAgua.CalcularValor(false);
        double valorEnergia = contaEnergia.CalcularValor(false);
        Console.Clear();
        Console.WriteLine("---------------------------------------");
        Console.WriteLine("Valor da conta de água do mês selecionado: R$" + Math.Round(valorAgua, 2));
        Console.WriteLine("Valor da conta de energia do mês selecionado: R$" + Math.Round(valorEnergia, 2));
        Console.WriteLine("---------------------------------------");

        Sleep(7000);
        month = 13;
        Console.Clear();

      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 1 a 12, de acordo com os meses.");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

  }

  static void CalcValueWithoutTax(Consumidor c)
  {
    int month = -1;
    while (month != 13)
    {
      try
      {
        Console.WriteLine("Insira o mês para realizar a conta pela conta em número de 1 a 12");
        month = int.Parse(Console.ReadLine());

        if (month < 1 || month > 12)
        {
          Console.WriteLine("Insira um mês válido");
        }

        ContaAgua contaAgua = c.Contas.OfType<ContaAgua>().FirstOrDefault(c => c.Mes == month);
        ContaEnergia contaEnergia = c.Contas.OfType<ContaEnergia>().FirstOrDefault(c => c.Mes == month);

        double valorAgua = contaAgua.CalcularValor(true);
        double valorEnergia = contaEnergia.CalcularValor(true);
        Console.Clear();
        Console.WriteLine("Valor da conta de água do mês selecionado: R$" + Math.Round(valorAgua, 2));
        Console.WriteLine("Valor da conta de energia do mês selecionado: R$" + Math.Round(valorEnergia, 2));

        Sleep(7000);
        month = 13;
        Console.Clear();

      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 1 a 12, de acordo com os meses.");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

  }
  static void AverageAcc(Consumidor c)
  {
    double mediaE = 0;
    double mediaA = 0;
    double sumA = 0;
    double sumE = 0;
    double countA = 0;
    double countE = 0;
    Console.Clear();
    try
    {
      foreach (Conta a in c.Contas)
      {
        if (a.Tipo == TipoConta.Agua)
        {
          sumA += a.CalcularValor(false);
          countA++;
        }
        else
        {
          sumE += a.CalcularValor(false);
          countE++;
        }
      }
      mediaE = sumE / countE;
      mediaA = sumA / countA;
      Console.WriteLine("------- Água -------");
      Console.WriteLine("O valor médio da conta de Água é: R$ " + Math.Round(mediaA, 2));
      Console.WriteLine("\n------- Energia -------");
      Console.WriteLine("O valor médio da conta de Energia é: R$ " + Math.Round(mediaE, 2));
      Console.WriteLine("---------------------------------------");
      Sleep(7000);
    }
    catch (Exception)
    {
      ErrorMessage("Erro na execução");
    }
  }

  static void MostValueMonth(Consumidor c)
  {
    Console.Clear();
    double maiorConsumoA = 0;
    double maiorConsumoE = 0;
    double maiorValorA = 0;
    double maiorValorE = 0;
    int mesCE = 0;
    int mesCA = 0;
    int mesVE = 0;
    int mesVA = 0;
    try
    {
      foreach (Conta a in c.Contas)
      {
        if (a.Tipo == TipoConta.Agua)
        {
          if (a.Consumo > maiorConsumoA)
          {
            maiorConsumoA = a.Consumo;
            mesCA = a.Mes;
          }
          if (a.CalcularValor(false) > maiorValorA)
          {
            maiorValorA = a.CalcularValor(false);
            mesVA = a.Mes;
          }
        }
        else
        {
          if (a.Consumo > maiorConsumoE)
          {
            maiorConsumoE = a.Consumo;
            mesCE = a.Mes;
          }
          if (a.CalcularValor(false) > maiorValorE)
          {
            maiorValorE = a.CalcularValor(false);
            mesVE = a.Mes;
          }
        }
      }
    }
    catch (Exception)
    {
      ErrorMessage("Erro na execução");
    }

    Console.WriteLine("------- Energia -------");
    Console.WriteLine("O maior Consumo foi de " + maiorConsumoE + " no mês " + mesCE);
    Console.WriteLine("O maior Valor foi de R$" + Math.Round(maiorValorE, 2) + " no mês " + mesVE);
    Console.WriteLine("\n------- Água -------");
    Console.WriteLine("O maior Consumo foi de " + maiorConsumoA + " no mês " + mesCA);
    Console.WriteLine("O maior Valor foi de R$" + Math.Round(maiorValorA, 2) + " no mês " + mesVA);

    Sleep(7500);
    Console.Clear();
  }
  static void VariationMonth(Consumidor c)
  {
    int month = -1;
    int month2 = -1;
    while (month != 0)
    {
      try
      {
        Console.WriteLine("Insira o primeiro mês para a comparação em número de 1 a 12");
        month = int.Parse(Console.ReadLine());
        Console.WriteLine("Insira o segundo mês para a comparação em número de 1 a 12");
        month2 = int.Parse(Console.ReadLine());
        if (month < 1 || month > 12 || month2 < 1 || month2 > 12)
        {
          if (month == 0 || month2 == 0)
            return;
          Console.WriteLine("Insira um mês válido");
        }

        ContaAgua contaAgua = c.Contas.OfType<ContaAgua>().FirstOrDefault(c => c.Mes == month);
        ContaAgua contaAgua2 = c.Contas.OfType<ContaAgua>().FirstOrDefault(c => c.Mes == month2);
        ContaEnergia contaEnergia = c.Contas.OfType<ContaEnergia>().FirstOrDefault(c => c.Mes == month);
        ContaEnergia contaEnergia2 = c.Contas.OfType<ContaEnergia>().FirstOrDefault(c => c.Mes == month2);

        double variaçãoEnergia = contaEnergia.CalcularValor(false) - contaEnergia2.CalcularValor(false) < 0 ? (contaEnergia.CalcularValor(false) - contaEnergia2.CalcularValor(false)) * -1 : contaEnergia.CalcularValor(false) - contaEnergia2.CalcularValor(false);
        double variacaoconsumoE = contaEnergia.Consumo - contaEnergia2.Consumo < 0 ? (contaEnergia.Consumo - contaEnergia2.Consumo) * -1 : contaEnergia.Consumo - contaEnergia2.Consumo;

        double variaçãoAgua = contaAgua.CalcularValor(false) - contaAgua2.CalcularValor(false) < 0 ? (contaAgua.CalcularValor(false) - contaAgua2.CalcularValor(false)) * -1 : contaAgua.CalcularValor(false) - contaAgua2.CalcularValor(false);
        double variacaoconsumoA = contaAgua.Consumo - contaAgua2.Consumo < 0 ? (contaAgua.Consumo - contaAgua2.Consumo) * -1 : contaAgua.Consumo - contaAgua2.Consumo;
        Console.Clear();
        Console.WriteLine("------- Água -------");
        Console.WriteLine("Variação da conta de Água dos meses " + month + " e " + month2 + " diferença de valor: R$ " + Math.Round(variaçãoAgua, 2) + " | diferença de consumo: " + variacaoconsumoA);
        Console.WriteLine("\n------- Energia -------");
        Console.WriteLine("Variação da conta de Energia dos meses " + month + " e " + month2 + " diferença de valor: R$ " + Math.Round(variaçãoEnergia, 2) + " | diferença de consumo: " + variacaoconsumoE);

        Sleep(7000);
        month = 0;
        Console.Clear();

      }
      catch (FormatException e)
      {
        ErrorMessage("Insira um número válido de 1 a 12, de acordo com os meses.");
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
      if (consumidorEncontrado.Contas.Count == 0)
      {
        Console.Clear();
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine("Consumidor procurado não tem nenhuma conta cadastrada!");
        Console.WriteLine("--------------------------------------------");

        Console.WriteLine("\nRetornando para o menu inicial...");
        Sleep(5000);
        return;
      }

      int entrada = -1;

      while (entrada != 6)
      {
        Console.Clear();
        Console.WriteLine("------ Consumidor com o cpf buscado: " + consumidorEncontrado.Nome + " ------");
        Console.WriteLine("0. Consumo de energia/água no último mês");
        Console.WriteLine("1. Valor total da conta");
        Console.WriteLine("2. Valor da conta sem impostos");
        Console.WriteLine("3. Variação de minha conta em reais e em consumo, entre dois meses escolhidos");
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
          case 1:
            CalcValueWithTax(consumidorEncontrado);
            break;
          case 2:
            CalcValueWithoutTax(consumidorEncontrado);
            break;
          case 3:
            VariationMonth(consumidorEncontrado);
            break;
          case 4:
            AverageAcc(consumidorEncontrado);
            break;
          case 5:
            MostValueMonth(consumidorEncontrado);
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

  static void WriteTheFile()
  {
    try
    {
      StreamWriter sw = new StreamWriter("consumidores.txt");

      foreach (Consumidor consumidor in consumidores)
      {
        sw.WriteLine(consumidor.IdConsumidor + "; " + consumidor.Nome + "; " + consumidor.Cpf + "; " + consumidor.TipoConsumidor);

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
      using StreamReader sr = new(name + ".txt");
      ProcessFileContent(sr);
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
          Console.Clear();
          Console.WriteLine("Insira um Cpf para buscar entre os consumidores");
          string cpf = Console.ReadLine();
          Search(cpf);
          Sleep(3000);
          break;

        case 2:
          Console.Clear();
          if (consumidores.Count > 0)
          {
            Console.WriteLine("------ Listagem de consumidores ------");
            foreach (Consumidor c in consumidores)
            {
              Console.WriteLine();
              Console.WriteLine("Consumidor " + c.IdConsumidor + ": " + c.Nome + " - " + c.TipoConsumidor);
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
              Console.WriteLine();
              Console.WriteLine("Consumidor " + c.IdConsumidor + ": " + c.Nome + " - " + c.TipoConsumidor);
              Console.WriteLine();
              foreach (Conta conta in c.Contas)
              {
                if (conta.Tipo == 0)
                {
                  Console.WriteLine("Água" + "; " + "Leitura atual: " + conta.LeituraAtual + "; Consumo: " + conta.Consumo);
                }
                else
                {
                  Console.WriteLine("Energia" + "; " + "Leitura atual: " + conta.LeituraAtual + "; Consumo: " + conta.Consumo);
                }

              }
              Console.WriteLine();
              Console.WriteLine("---------------------------------------");
            }
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
