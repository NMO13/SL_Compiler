PROGRAM
	VAR i, j : INTEGER;
    PROCEDURE Foo(VAR i, j, k, l, m, n, o : INTEGER; VAR p, q : CHAR) : INTEGER;
	BEGIN
		RETURN i;
	END Foo;
BEGIN
	i := Foo(2, 3, 4, 5, 6, 7, 8, "s", "w");
	Assert(i, 2);
	j := Foo(6, 3, 4, 5, 6, 7, 8, "s", "w");
	Assert(j, 6)
END.
