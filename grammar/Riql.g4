grammar Riql;

/**************
 * Parser Rules
 */

start:
	(
		K_WHERE where
		| K_ORDERBY orderby
		| K_REDUCE reduce
		| K_SKIP skip
		| K_TAKE take
	)* EOF;

where: {UseFullText(true);} any*? {UseFullText(false);};
propertyPath: (property) (DOT property)*;
property: IDENTIFIER | nonbreakingkeyword;

orderby: propertyPath (K_ASC | K_DESC)? (COMMA orderby)*;

reduce: property (',' property)*;

skip: INT;

take: INT;

any:
	IDENTIFIER
	| INT
	| K_ASC
	| K_DESC
	| SINGLE_QUOTE
	| DOUBLE_QUOTE
	| DOT
	| COMMA
	| WHITESPACE
	| ANY
	| nonbreakingkeyword;

nonbreakingkeyword: K_ASC | K_DESC;
breakingkeyword:
	K_WHERE
	| K_ORDERBY
	| K_REDUCE
	| K_SKIP
	| K_TAKE;

/*************
 * Lexer Rules
 */

// Case insensitive alphabet
fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];
fragment DIGIT: [0-9];
INT: '-'? DIGIT+;

K_ASC: A S C;
K_DESC: D E S C;

K_WHERE: KEYMARKER W (H E R E)? '=';
K_ORDERBY: KEYMARKER O (R D E R B Y)? '=';
K_REDUCE: KEYMARKER R (E D U C E)? '=';
K_SKIP: KEYMARKER S (K I P)? '=';
K_TAKE: KEYMARKER T (A K E)? '=';

SINGLE_QUOTE: '\'' ('\\\'' | ~('\''))* '\'';
DOUBLE_QUOTE: '"' ('\\"' | ~('"'))* '"';

KEYMARKER: '$';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
DOT: '.';
COMMA: ',';
WHITESPACE: [ '\t] { if (!UseFullText) Skip(); };
ANY: .;