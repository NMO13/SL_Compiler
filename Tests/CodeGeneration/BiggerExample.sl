PROGRAM
VAR i: INTEGER;
	PROCEDURE PutInt(x: INTEGER); /* groesste ausgebbare Zahl = 9999 */
	VAR c0, c1, c2, c3: CHAR;
	BEGIN
		c3 := CHR(48 + x % 10); x := x / 10;
		c2 := CHR(48 + x % 10); x := x / 10;
		c1 := CHR(48 + x % 10); x := x / 10;
		c0 := CHR(48 + x % 10);
		IF c0 > "0" THEN Put(c0); Put(c1); Put(c2)
		ELSIF c1 > "0" THEN Put(c1); Put(c2)
		ELSIF c2 > "0" THEN Put(c2)
		END;
		Put(c3)
		END PutInt;
	BEGIN /* ungerade Zahlen ausgeben */
		i := 1;
		WHILE i < 100 DO
		PutInt(i); PutLn();
		i := i + 2
	END
END.