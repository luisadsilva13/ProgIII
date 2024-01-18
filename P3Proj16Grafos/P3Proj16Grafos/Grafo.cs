﻿using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;

namespace P3Proj16Grafos
{
	/// <summary>
	/// Description of Grafo.
	/// Classe para simular estrutura de dados do tipo grafo
	/// Também aqui deveriamos usar generics para tornar a nossa classe mais abrangente
	/// no entanto, para simplificar o estudo vamos usar:
	/// - string para identificar os diferentes vertícies:
	/// - int para as ligações (apenas valores positivos; 0 ou <0 para inexistente...)
	/// </summary>
	public class Grafo
	{
     // lista de cores... para desenhar grafo
     static KnownColor[] colorNames  = (KnownColor[])Enum.GetValues(typeof(KnownColor));

     private int totalVertices=0;        // guarda número de vétices do grafo
	 public string[] vertices=null;      // lista de nomes dos vértices do Grafo
	 public int[,] ramos=null;          // guardar informação das ligações (int para testes deveria usar-se generics)

	 public Point[] posicoes=null;		// guarda as coordenadas para representação gráfica (para nosso uso)
		
#region Construtor da Classe
		public Grafo(int NumeroDeVertices)
		{		
		 this.vertices = new string[NumeroDeVertices];
		 this.ramos = new int[NumeroDeVertices,NumeroDeVertices];
		 this.totalVertices=NumeroDeVertices;		 
		 this.posicoes = new Point[NumeroDeVertices];
		 
		 for (int i=0;i<this.totalVertices;i++)
		 	{
		 	 this.vertices[i]="Vertice " + i;
		 	}
		 		 
		 
		}
#endregion
		
#region Propriedades da Classe
    // devolve o número de vétices que constituem o Grafo
	public int TotalVertices
	{get {
		 return this.totalVertices;	
		}
	}

	// Propriedade que calcula e devolve o número de ramos válidos no grafo
	// lembrar existe ligação de valor na matriz >0
	// Ordem de complexidade O(N^2)
	public int TotalRamos
	{get {int c=0;
		  // percorrer a matriz e contar as ligações
		  for (int i=0;i<this.TotalVertices;i++)
		   {
		  	 for (int j=0;j<this.totalVertices;j++)
		  	   {
		  	 	if (this.ramos[i,j]>0)	// se existe ligação Vi pra Vj
		  	 		c++;
		  	   }// fim for colunas
		  } // fim for linhas
		  return c;		
		}
	}
	
	// propriedade que devolve a densidade do Grafo: R/(V*(V-1))
	public double Densidade
	{get {
		  return (double)this.TotalRamos/(this.totalVertices*(this.totalVertices-1));
		 }		
	}
	
#endregion
		

#region Métodos Públicos dos objectos da Classe
   // devolve o Grafo formatado em forma de matriz de adjacência
   // para colocar numa textBox qualquer...
   // cada linha tem 13 espaços + 5*numvertices + 2 caracteres (mudar de linha) -- Não mexer!!!!
   public override string ToString()
	{string saida="             ";
   	 // primera linha  .... 00  01  20  03
   	 for (int i=0;i<this.totalVertices;i++)
   	 	saida=saida+String.Format(": {0:00} ",i);
   	 saida = saida + "\r\n";    // mudar de linha
   	 // linhas seguintes:
   	 for (int i=0;i<this.totalVertices;i++)
   	 {saida = saida + String.Format("{0,-11}{1:00}",(this.vertices[i].Length>11)?this.vertices[i].Substring(0,11):this.vertices[i].ToString(),i);
   	  for (int j=0;j<this.totalVertices;j++)
   	  	if (this.ramos[i,j].ToString().Length>4)
   	  	   saida=saida+String.Format(":{0,3}+",this.ramos[i,j].ToString().Substring(0,3));
   	    else
   	    	saida=saida+String.Format(":{0,4}",(this.ramos[i,j]<=0)?"":this.ramos[i,j].ToString());
   	  saida = saida + "\r\n";
   	   }
   	 return saida.Substring(0,saida.Length-2); // devolver tirando o último \r\n
	}

   
   // método que tenta desenhar o grafo de forma gráfica numa Image criada com tamanho comprimentoxaltura
   public Image ToImage(int comprimento, int altura)
	{
	 if (comprimento<=0 || altura<=0)
			return null;
		 
	 Pen pen1 = new Pen(Color.DarkSalmon,2);		 
	 Font font = new System.Drawing.Font("Currier New", 9,FontStyle.Regular, GraphicsUnit.Point);
	 StringFormat f = new StringFormat();
	 f.Alignment= StringAlignment.Center;

	 Image imag = new Bitmap(comprimento,altura,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
	 Graphics g=Graphics.FromImage(imag);
	 g.Clear(Color.LightGray);
	 //g.DrawRectangle(pen1,1,1,imag.Width-1,imag.Height-1);
	 if (this.vertices==null)
	 	  return imag; 		 
	 
	 AdjustableArrowCap bigArrow = new AdjustableArrowCap(4, 8);	 		 
	 // desenhar os ramos
     Pen pen2 = new Pen(Color.DarkRed,2); 
     pen2.CustomEndCap = bigArrow;
	 for (int i=0;i<this.totalVertices;i++)
	      {
	 	   int px=this.posicoes[i].X;
	 	   int py=this.posicoes[i].Y;
	 	   pen2.Color=Color.FromKnownColor(colorNames[(i*3)%colorNames.Length]);
	       for (int j=0;j<this.totalVertices;j++)
	       	if (this.ramos[i,j]>0)
	           {g.DrawLine(pen2,px,py,this.posicoes[j].X,this.posicoes[j].Y);
	       	    g.DrawString(this.ramos[i,j].ToString(),font,Brushes.Blue,(px+this.posicoes[j].X)/2+1,(py+this.posicoes[j].Y)/2-5,f);
	           }
		  }

	 // Agora desenhar por cima os vertíces
	 for (int i=0;i<this.totalVertices;i++)
	      {
	 	   g.DrawArc(pen1,this.posicoes[i].X-10,this.posicoes[i].Y-10,20,20,0,360);
	 	   g.DrawString(this.vertices[i]+"("+i+")",font,Brushes.Blue,this.posicoes[i].X,this.posicoes[i].Y-10,f);
		  }

	 g.Dispose();
	 return imag; 		 
	}
   
   
   // método que desenha numa imagem o trajeto contido no vertor vert
   public Image DrawInImage(int[] vert, Image img)
   {
   	if (vert==null || img==null || this.totalVertices==0)
   		return null;
		 
	 Pen pen1 = new Pen(Color.Yellow,4);		 
     pen1.EndCap = LineCap.ArrowAnchor;
	 Graphics g=Graphics.FromImage(img);
	 // desenhar as ligações
	 for (int i=0;i<vert.Length-1;i++)
	      {
	 	   g.DrawLine(pen1,this.posicoes[vert[i]].X,this.posicoes[vert[i]].Y,this.posicoes[vert[i+1]].X,this.posicoes[vert[i+1]].Y);
		  }
	 g.Dispose();
	 return img; 		 
	}
   
   // método que faz a gravação dos dados de um grafo em disco
   // usando um formato próprio....
   // 1ª linha: texto GrafoP3
   // 2ª linha: total de vertices (N)
   // N linhas: nomes dos vertices
   // N linhas: matriz de ligação (valores separados por espaços e com * onde não há ligações)
   public void ToFile(string nomeF)
   {
   	StreamWriter fs = new StreamWriter(nomeF,false,System.Text.Encoding.GetEncoding("iso-8859-1"));
   	fs.WriteLine("GrafoP3");
   	fs.WriteLine(this.totalVertices.ToString());
   	// colocar no ficheiro o nome dos vertices e suas coordenadas...
   	for (int i=0;i<this.totalVertices;i++)
   		fs.WriteLine("{0};{1};{2}",this.vertices[i],this.posicoes[i].X,this.posicoes[i].Y);
   	// colocar no ficheiro os dados das ligações
   	for (int i=0;i<this.totalVertices;i++)
   	    {for (int j=0;j<this.totalVertices;j++)
   		    if (this.ramos[i,j]<=0)
   		         fs.Write("* ");
   	          else
   	          	 fs.Write("{0} ",this.ramos[i,j].ToString());
   	     fs.WriteLine();
    	}
   	fs.Close();
   }

   // método que calcula e devolve o grau de um vértice
   // faz uso de outros métodos para o calculo...
   public int GrauVertice(int vert)
   {
   	return this.TotalSucessores(vert) + this.TotalAntecessores(vert);
   }
   
   /// <summary>
   /// método que devolve um vector com os índices dos vértices sucessores de Vini
   /// </summary>
   /// <param name="vini">Indice do vertice inicial onde começa a listagem</param>
   /// <returns>vector contendo os indices dos vertices</returns>
   public int[] Sucessores(int vini)
   {
   	if (vini<0 || vini>=this.TotalVertices)
   		return null;
   	List<int> lstsuc = new List<int>();
   	//  preencher lista com sucessores..
   	for (int j=0;j<this.totalVertices;j++)
   		if (this.ramos[vini,j]>0)
   			{
	   		 lstsuc.Add(j);
   			}
   	return lstsuc.ToArray(); // devolve a lista transformada em vetor
   }   
   /// <summary>
   /// método que devolve um vector com os índices dos vértices Antecessores de Vini
   /// </summary>
   /// <param name="vini">Indice do vertice inicial onde começa a listagem</param>
   /// <returns>vector contendo os indices dos vertices</returns>
   public int[] Antecessores(int vini)
   {if (vini<0 || vini>=this.TotalVertices)
   		return null;   
   	int[] vector= new int[this.TotalAntecessores(vini)];
   	// preencher este vector com os indices dos antecessores de vini
   	int k=0;
   	for (int i=0;i<this.totalVertices;i++)
   		if (this.ramos[i,vini]>0)	// se i for antecessor de vini
   			{
   			 vector[k]=i;
   			 k++;
   			}
   	return vector;
   }
   
   /// <summary>
   /// método que permite obter a lista de nomes de vertices (na forma de string), separados por ;
   /// </summary>
   /// <param name="vecindices">vector de índices a saber o nome</param>
   /// <returns>string contendo a lista de nomes sparada por ;</returns>
   public string IndicesToNomes(int[] vecIndices)
   {string saida="";
   	for (int i=0;i<vecIndices.Length;i++)
   		saida=saida + this.vertices[vecIndices[i]] + " ; ";
   	return saida;
   }
   
   /// <summary>
   /// Implementação da travessia do grafo 
   /// usando como estratégia a pesquisa em Largura (BreadthFirst)
   //  devolve um vector contendo os vértices visitados ao longo dessa travessia
   /// </summary>
   /// <param name="vi">Indice do vertice onde começa a travessia</param>
   /// <returns>lista (vector) de vertices atravessados por esta travessia</returns>
   public int[] BreadthFirst(int vi)
   {
   	if (vi<0 || vi>=this.totalVertices)
   		return null;
   	bool[] visitados = new bool[this.totalVertices];// para marcar visitados
   	List<int> lista = new List<int>();		  // para colocar travessia..
   	Queue<int> fila = new Queue<int>();   // guardar temporaiamente os valores
   	fila.Enqueue(vi);	// começar a travessia por vi
   	visitados[vi]=true; // marcar este vertice como já processado
   	while (fila.Count>0)
   		{
   		 int v=fila.Dequeue();	// retirar próximo elemento da fila
   		 lista.Add(v);			// acrescentar este vertice à saída
   		 // colocar agora todos os sucessores (não visitados) de v na fila 
   		 for (int j=0;j<this.totalVertices;j++)
   		 	if (this.ramos[v,j]>0 && visitados[j]==false)
   		 			{
   		 			 fila.Enqueue(j);	// colocar j na fila
   		 			 visitados[j]=true;	// marcar j como processado
   		 			}
   		} // fim do while
	return lista.ToArray(); // transforma a lista em um array e devolve-o
   }
   
   /// <summary>
   /// Implementação da travessia do grafo 
   /// usando como estratégia a pesquisa em Profundidade (usa recursividade)
   //  devolve um vector contendo os vértices visitados ao longo dessa travessia
   /// </summary>
   /// <param name="vi">Indice do vertice onde começa a travessia</param>
   /// <returns>lista (vector) de vertices atravessados por esta travessia</returns>
   public int[] DepthFirst(int vi)
   {
   	if (vi<0 || vi>=this.totalVertices)
   		return null;
   	bool[] visitados= new bool[this.totalVertices];
   	List<int> lista = new List<int>();
   	DepthFirstRecursivo(vi,visitados,lista);
   	return lista.ToArray();
   }
   
   // método que verifica se existe um qualquer caminho entre dois vértices do grafo
   // devolve true em caso afirmativo.
   public bool ExisteCaminho(int vini, int vfim)
   {
   	bool[] visitados = new bool[this.totalVertices];   
   	return Ha_Caminho(vini,vfim,visitados);
   }

   // método que verifica a existencia de um caminho entre dois vértices 
   // devolvendo-o na forma de vector (null caso não exista)
   public int[] UmCaminho(int vini, int vfim)
   {   	
   	bool[] visitados = new bool[this.totalVertices];
   	List<int> lista = new List<int>();   
   	if (Ha_Caminho(vini,vfim,visitados,lista)==true)
   		lista.Add(vini);
   	lista.Reverse();
   	return lista.ToArray();   	   	
   }
   
   /// <summary>
   /// método que calcula qual o menor caminho entre dois vétices
   /// implementa o algoritmo de dijkstra
   /// </summary>
   /// <param name="vini">Indice do vertice partida</param>
   /// <param name="vfim">Indice do vertice destino</param>
   /// <returns>devolve o caminho a percorrer na forma de vector (null caso não exista caminho possível)</returns>
   public int[] MenorCaminho(int vini, int vfim)
   {
   	bool[] visitado = new bool[this.totalVertices]; 
   	int[] distancia = new int[this.totalVertices];
   	int[] anterior = new int[this.totalVertices]; //para poder saber o caminho "de volta"   	
   	// iniciar vetores
   	for (int i=0;i<this.totalVertices;i++)
   	  {visitado[i]=false;
   	   anterior[i]=vini;
   	   if (this.ramos[vini,i]>0) // se i sucessor de vini
   	   	  distancia[i]=this.ramos[vini,i];
   	   	else				
   	      distancia[i]=100000;  // se não for então oo
   	  }
   	// algoritmo propriamente dito...
   	distancia[vini]=0;
   	visitado[vini]=true;
	 //...
	 //...
	 //...
	 //...
	 //...
	 //...		 
		 
   	// no fim do while temos em anterior o caminho mais curto para todos os vert.
   	// vamos agora reconstruir o caminho de vini para vfim
   	if (distancia[vfim]==100000)
   		return null;				// não existe caminho

   	Stack<int> caminho = new Stack<int>();
   	caminho.Push(vfim);
   	int ant=vfim;
   	while (anterior[ant]!=vini)
   	   {
   		ant=anterior[ant];
   		caminho.Push(ant);
   	   }
   	caminho.Push(vini);
   	   	
   	return caminho.ToArray();
   }
   
   /// <summary>
   /// método que transforma o grafo actual num grafo de menores distâncias de todos para todos
   /// faz uso do algortimo de Floyd-Warshall para este objectivo
   /// </summary>
   public void ToGrafoMenoresDistancias()
   {int[,] dist = new int[this.totalVertices,this.totalVertices];
   	//iniciar distancias
   	for (int i=0;i<this.totalVertices;i++)
   	   for (int j=0;j<this.totalVertices;j++)
   		   dist[i,j]=(this.ramos[i,j]>0)?this.ramos[i,j]:1000000;   		
   	// algoritmo propriamente dito
		 //...
		 //...
		 //...
    // colocar o resultado em ramos deste grafo (copiar dist para ramos)
   	for (int i=0;i<this.totalVertices;i++)
   	   for (int j=0;j<this.totalVertices;j++)
   		  this.ramos[i,j]=(dist[i,j]==1000000)?0:dist[i,j];
   }
   
   // método que transforma o grafo actual numa árvore geradora minima
   // faz uso do algortimo de Prim para este objectivo
   public void ToArvoreGeradoraMinima(int vini)
   {
   	bool[] visitado = new bool[this.totalVertices]; 
   	int[] distancia = new int[this.totalVertices];
   	int[] anterior = new int[this.totalVertices]; //para poder reconstruir o grafo
   	for (int i=0;i<this.totalVertices;i++)
   	   {
   		distancia[i]=100000;
   	   }
   	// algoritmo propriamente dito
   	anterior[vini]=-1;
   	distancia[vini]=0;
   	//...
    //...
	//...
    // agora temos que reconstruir o grafo com a cobertura minima dada por anterior
	this.ramos= new int[this.totalVertices,this.totalVertices]; // nova matriz
	for (int i=0;i<distancia.Length;i++)
		if (distancia[i]!=0 && distancia[i]!=100000)
			this.ramos[i,anterior[i]]=this.ramos[anterior[i],i]=distancia[i];
   }
   
   /// <summary>
   /// Método que calcula e devolve a lista de vertinices inacessíveis
   /// a partir de um vertice inical.
   /// </summary>
   /// <param name="vini">vértice inicial</param>
   /// <returns>lista contendo indices inacessíves. Null caso não exista</returns>
   public int[] Inacessiveis(int vini)
   {
   	//...
    //...
	//...
   	return null;
   }
   #endregion

#region Métodos privados da classe
    // Método auxiliar da travessia em Profundidade...
    private void DepthFirstRecursivo(int vi,bool[] visit, List<int> lista)
    {
     visit[vi]=true;		// marca-lo como visitado
     lista.Add(vi);			// acrescenta-lo à saída..
     // Na lista de sucessores (não visitados) voltar a chamar este mesmo
     // método de forma recursiva...
     for (int j=0;j<this.totalVertices;j++)
     	if (this.ramos[vi,j]>0 && visit[j]==false)
     		{
	     	 DepthFirstRecursivo(j,visit,lista);
     		}
    }
    

	/// <summary>
    /// Método auxiliar para verificar se existe caminho entre dois vértices..
    /// Faz uso da definição de caminho...
	/// </summary>
	/// <param name="vini">indice do vertice inicial</param>
	/// <param name="vfim">indice do vertice final</param>
	/// <param name="visit">booleanos para assinalar já visitados</param>
	/// <returns></returns>
    private bool Ha_Caminho(int vini, int vfim,bool[] visit)
    {
     visit[vini]=true;
     // existe caminho de vini para vfim se houver ramo vini->vfim	
     if (this.ramos[vini,vfim]>0)		
     	   return true;
     //ou então se exitir caminho de um dos sucessores para vfim
     for (int j=0;j<this.totalVertices;j++)
     	if (this.ramos[vini,j]>0 && visit[j]==false)
     		{
	     	 bool res=Ha_Caminho(j,vfim,visit);
	     	 if (res==true)
	     	 	return true;
     		}
     return false;						// por aqui não há caminho...
    }    
    
    
	/// <summary>
    /// Método auxiliar para verificar se existe caminho entre dois vértices (
    /// caso exista coloca esse caminho numa lista ligada
    /// Faz uso da definição de caminho...
	/// </summary>
	/// <param name="vini">indice do vertice inicial</param>
	/// <param name="vfim">indice do vertice final</param>
	/// <param name="visit">boolenos para assinalar já visitados</param>
	/// <param name="lista">lista ligada contendo caminho </param>
	/// <returns></returns>
    private bool Ha_Caminho(int vini, int vfim,bool[] visit, List<int> lista)
    {
     visit[vini]=true;
     // existe caminho de vini para vfim se houver ramo vini->vfim	
     if (this.ramos[vini,vfim]>0)		
          {
     	   lista.Add(vfim);
     	   return true;
          }
     //ou então se exitir caminho de um dos sucessores de vini para vfim
     for (int j=0;j<this.totalVertices;j++)
     	if (this.ramos[vini,j]>0 && visit[j]==false)
     		{
	     	 bool res=Ha_Caminho(j,vfim,visit,lista);
	     	 if (res==true)
	     	 {
	     	  lista.Add(j);
	     	  return true;
	     	 }
     		}
     return false;						// por aqui não há caminho...
    }
      
	// método que devolve o número de sucessores de um vértice
	// complexidade O(N)
	private int TotalSucessores(int vini)
	{int c=0; 
	// percorrer a "linha" vini e contar as suas ligações
	 for (int j=0;j<this.totalVertices;j++)
	 	if (this.ramos[vini,j]>0)
				c++;
	 return c;
	}

	// método que devolve o número de Antecessores de um vértice
	// complexidade O(N)
	private int TotalAntecessores(int vini)
	{int c=0;
   	// percorrer a "coluna" vini e contar as suas ligações
   	for (int i=0;i<this.totalVertices;i++)
   		if (this.ramos[i,vini]>0)
   			c++;
	 return c;
	}

	// método que devolve o indice da menor distância (>0) de entre os vértices não visitados
	// devolve -1 caso já esteja tudo visitado
	private static int IndMenorDistanciaNVisitado(int[] dist, bool[] visit)
	{int ind=-1;
	 int menor=100000;
	 for (int i=0;i<dist.Length;i++)
	 	if (dist[i]<menor && visit[i]==false)
	        {menor=dist[i];
	 	     ind=i;
	        }
	 return ind;	
	}
#endregion

#region Métodos static da classe Grafo 
    // método que cria um grafo com os dados guardados num ficheiro 
    // formatado de formaespecial (1-GtafoP3, 2-numvertices, 3-nomes, 4-ramos....)
    public static Grafo GrafoFromFile(string nomeF)
    {Grafo grafo=null;
     string[] linhas;
     linhas=File.ReadAllLines(nomeF,System.Text.Encoding.GetEncoding("iso-8859-1")); //ler todos os dados do ficheiro (linha a linha)
     if (linhas.Length<6)				// Não deve ser um grafo (falta mostra mensagem...)
	 	      return null;					
     if (linhas[0]!="GrafoP3")  		// Ver 1ª linha: Não deve ser um grafo nosso (falta mostra mensagem...)
     	 return null;			
     int numvertices;
     int.TryParse(linhas[1],out numvertices);
     if (numvertices<2)
     	 return null;
     grafo = new Grafo(numvertices);   // reservar espaço para este grafo
     // ler as informações dos seus vertices..
     for (int i=0;i<numvertices;i++)
      {string[] umalinha = linhas[i+2].Split(';');
     	 if (umalinha.Length<3)
     	        return null;	
       grafo.vertices[i]=umalinha[0];
       int px,py;
       int.TryParse(umalinha[1],out px);grafo.posicoes[i].X=px;
       int.TryParse(umalinha[2],out py);grafo.posicoes[i].Y=py;
      }
     // tentar agora ler informções sobre as ligações entre vertices
     for (int i=0;i<numvertices;i++)
        {string[] umalinha = linhas[numvertices+i+2].Split(' ');
     	 if (umalinha.Length<numvertices)
     	        return null;	
     	 for (int j=0;j<numvertices;j++)
     	 	int.TryParse(umalinha[j],out grafo.ramos[i,j]);
        }     	
	 return grafo;
    }
#endregion
	}
}
