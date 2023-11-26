public partial class Conta
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
  public double CalcularValor(bool imposto)
  {
    try
    {
      if (Tipo == TipoConta.Agua)
      {
        if (Consumidor.TipoConsumidor == TipoImovel.Comercial)
        {
          if (imposto)
          {
            if (Consumo < 6)
            {
              return 25.79 + 12.90;
            }
            else if (Consumo >= 6 && Consumo < 10)
            {
              return Consumo * 4.299 + Consumo * 2.149;
            }
            else if (Consumo >= 10 && Consumo < 40)
            {
              double valorConsumoAntigo = 10 * 4.299 + 10 * 2.149;

              double restoConsumo = Consumo - 10;
              double valorConsumo = restoConsumo * 8.221 + restoConsumo * 4.111;
              return valorConsumoAntigo + valorConsumo;
            }
            else if (Consumo >= 40 && Consumo < 100)
            {
              double valorConsumo0 = 10 * 4.299 + 10 * 2.149;

              double ValorConsumo1 = 30 * 8.221 + 30 * 4.111;

              double restoConsumo = Consumo - 40;

              double valorConsumo = restoConsumo * 8.288 + restoConsumo * 4.144;

              return valorConsumo0 + ValorConsumo1 + valorConsumo;
            }
            else
            {
              double valorConsumo0 = 10 * 4.299 + 10 * 2.149;

              double valorConsumo1 = 30 * 8.221 + 30 * 4.111;

              double valorConsumo2 = 60 * 8.288 + 60 * 4.144;

              double restoConsumo = Consumo - 100;

              double valorConsumo = restoConsumo * 8.329 + restoConsumo * 4.165;
              return valorConsumo + valorConsumo0 + valorConsumo1 + valorConsumo2;
            }
          }
          else
          {
            if (Consumo < 6)
            {
              return 25.79 + 12.90;
            }
            else if (Consumo >= 6 && Consumo < 10)
            {
              return (Consumo * 4.299 + Consumo * 2.149) * 1.03;
            }
            else if (Consumo >= 10 && Consumo < 40)
            {
              double valorConsumoAntigo = 10 * 4.299 + 10 * 2.149;

              double restoConsumo = Consumo - 10;
              double valorConsumo = restoConsumo * 8.221 + restoConsumo * 4.111;
              return (valorConsumoAntigo + valorConsumo) * 1.03;
            }
            else if (Consumo >= 40 && Consumo < 100)
            {
              double valorConsumo0 = 10 * 4.299 + 10 * 2.149;

              double ValorConsumo1 = 30 * 8.221 + 30 * 4.111;


              double restoConsumo = Consumo - 40;

              double valorConsumo = restoConsumo * 8.288 + restoConsumo * 4.144;

              return (valorConsumo0 + ValorConsumo1 + valorConsumo) * 1.03;
            }
            else
            {
              double valorConsumo0 = 10 * 4.299 + 10 * 2.149;

              double valorConsumo1 = 30 * 8.221 + 30 * 4.111;

              double valorConsumo2 = 60 * 8.288 + 60 * 4.144;

              double restoConsumo = Consumo - 100;

              double valorConsumo = restoConsumo * 8.329 + restoConsumo * 4.165;
              return (valorConsumo + valorConsumo0 + valorConsumo1 + valorConsumo2) * 1.03;
            }
          }
        }
        else
        {
          if (imposto)
          {
            if (Consumo < 6)
            {
              return 10.08 + 5.05;
            }
            else if (Consumo >= 6 && Consumo < 10)
            {
              return Consumo * 2.241 + Consumo * 1.122;
            }
            else if (Consumo >= 10 && Consumo < 15)
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double restoConsumo = Consumo - 10;

              double valorConsumo = restoConsumo * 5.447 + restoConsumo * 2.724;

              return valorConsumo0 + valorConsumo;
            }
            else if (Consumo >= 15 && Consumo < 20)
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double valorConsumo1 = 5 * 5.447 + 5 * 2.724;

              double restoConsumo = Consumo - 15;

              double valorConsumo = restoConsumo * 5.461 + restoConsumo * 2.731;
              return valorConsumo0 + valorConsumo1 + valorConsumo;
            }
            else if (Consumo >= 20 && Consumo < 40)
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double valorConsumo1 = 5 * 5.447 + 5 * 2.724;
              double valorConsumo2 = 5 * 5.46 + 5 * 2.731;


              double restoConsumo = Consumo - 20;

              double valorConsumo = restoConsumo * 5.487 + restoConsumo * 2.744;
              return valorConsumo0 + valorConsumo1 + valorConsumo2 + valorConsumo;
            }
            else
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double valorConsumo1 = 5 * 5.447 + 5 * 2.724;
              double valorConsumo2 = 5 * 5.46 + 5 * 2.731;
              double valorConsumo3 = 20 * 5.487 + 20 * 2.744;

              double restoConsumo = Consumo - 40;

              double valorConsumo = restoConsumo * 10.066 + restoConsumo * 5.035;
              return valorConsumo + valorConsumo0 + valorConsumo1 + valorConsumo2 + valorConsumo3;
            }
          }
          else
          {
            if (Consumo < 6)
            {
              return 10.08 + 5.05;
            }
            else if (Consumo >= 6 && Consumo < 10)
            {
              return (Consumo * 2.241 + Consumo * 1.122) * 1.03;
            }
            else if (Consumo >= 10 && Consumo < 15)
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double restoConsumo = Consumo - 10;

              double valorConsumo = restoConsumo * 5.447 + restoConsumo * 2.724;

              return (valorConsumo0 + valorConsumo) * 1.03;
            }
            else if (Consumo >= 15 && Consumo < 20)
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double valorConsumo1 = 5 * 5.447 + 5 * 2.724;

              double restoConsumo = Consumo - 15;

              double valorConsumo = restoConsumo * 5.461 + restoConsumo * 2.731;
              return (valorConsumo0 + valorConsumo1 + valorConsumo) * 1.03;
            }
            else if (Consumo >= 20 && Consumo < 40)
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double valorConsumo1 = 5 * 5.447 + 5 * 2.724;
              double valorConsumo2 = 5 * 5.46 + 5 * 2.731;


              double restoConsumo = Consumo - 20;

              double valorConsumo = restoConsumo * 5.487 + restoConsumo * 2.744;
              return (valorConsumo0 + valorConsumo1 + valorConsumo2 + valorConsumo) * 1.03;
            }
            else
            {
              double valorConsumo0 = 10 * 2.241 + 10 * 1.122;
              double valorConsumo1 = 5 * 5.447 + 5 * 2.724;
              double valorConsumo2 = 5 * 5.46 + 5 * 2.731;
              double valorConsumo3 = 20 * 5.487 + 20 * 2.744;

              double restoConsumo = Consumo - 40;

              double valorConsumo = restoConsumo * 10.066 + restoConsumo * 5.035;
              return (valorConsumo + valorConsumo0 + valorConsumo1 + valorConsumo2 + valorConsumo3) * 1.03;
            }
          }
        }
      }
      else
      {
        if (Consumidor.TipoConsumidor == TipoImovel.Comercial)
        {
          if (imposto)
          {
            return Consumo * 0.41 + 13.25;

          }
          else
          {
            return (Consumo * 0.41 + 13.25) * 1.2195;
          }
        }
        else
        {
          if (imposto)
          {
            return Consumo * 0.46 + 13.25;
          }
          else
          {
            if (Consumo < 90)
              return Consumo * 0.46 + 13.25;
            else
              return (Consumo * 0.46 + 13.25) * 1.4285;
          }
        }
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      return 0;
    }
  }
}

public partial class Consumidor
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

public enum TipoConta
{
  Agua = 0,
  Energia = 1,
}

public enum TipoImovel
{
  Residencial,
  Comercial
}