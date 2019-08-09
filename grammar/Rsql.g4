grammar Rsql;

/**************
 * Parser Rules
 */

selector:
	~(GROUP_START | GROUP_END | ';' | ',' | '=' | '<' | '>' | '!' | '\'' | '"')+;

start: or EOF;

or: and (',' and)*;

and: constraint (';' constraint)*;

constraint: group | comparison;

group: GROUP_START or GROUP_END;

comparison: selector comparator arguments;

comparator: (
		'=='
		| '=eq='
		| '!="'
		| '=neq='
		| '<'
		| '=lt='
		| '<='
		| '=le='
		| '>'
		| '=gt='
		| '>='
		| '=ge='
		| '=is-null='
		| '=nil='
		| '=in='
		| '=out='
		| '=nin='
	);

arguments: (GROUP_START value ( ',' value)* GROUP_END) | value;

value:
	~(GROUP_START | GROUP_END | ';' | ',' | '=' | '<' | '>' | '!' | '\'' | '"')+
	| SINGLE_QUOTE
	| DOUBLE_QUOTE;

/*************
 * Lexer Rules
 */

SINGLE_QUOTE: '\'' ('\\\'' | ~('\''))* '\'';
DOUBLE_QUOTE: '"' ('\\"' | ~('"'))* '"';
WHITESPACE: [ \t] -> channel(HIDDEN);
GROUP_START: '(';
GROUP_END: ')';
ANY: .;