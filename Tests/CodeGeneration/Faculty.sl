PROGRAM
VAR i: INTEGER;
	PROCEDURE Fac(x: INTEGER) : INTEGER;
	VAR temp : INTEGER;
	BEGIN
		IF x = 0 THEN
			RETURN 1
		ELSE
			temp := Fac(x-1);
			RETURN x * temp
		END;
	END Fac;
BEGIN
	i := Fac(10);
	Assert(i, 3628800);
END.