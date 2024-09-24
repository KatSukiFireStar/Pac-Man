public class Edge
{
	private Node from;
	private Node to;
	private int value;

	public Node From
	{
		get => from;
	}

	public Node To
	{
		get => to;
	}

	public int Value
	{
		get => value;
	}
	
	public Edge(Node from, Node to)
	{
		this.from = from;
		this.to = to;
		this.value = 1;
	}
}
