namespace Gateway.Exceptions;

public class CircuitOpenException(string message) : Exception(message);
