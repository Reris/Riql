using System;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Riql.Transpiler;
using Riql.Transpiler.Rsql;

namespace Riql
{
    public static class Riql
    {
        public static PropertyAccessor DefaultPropertyAccessor = new PropertyAccessor();

        public static IQueryable<T> ApplyRiql<T>(this IQueryable<T> query, string? riql, int maxTake = -1)
        {
            return query.ApplyRiql(riql, Riql.DefaultPropertyAccessor, maxTake);
        }

        public static IQueryable<T> ApplyRiql<T>(
            this IQueryable<T> query,
            string? riql,
            PropertyAccessor propertyAccessor,
            int maxTake = -1)
        {
            query = query ?? throw new ArgumentNullException(nameof(query));
            propertyAccessor = propertyAccessor ?? throw new ArgumentNullException(nameof(propertyAccessor));
            if (string.IsNullOrWhiteSpace(riql))
            {
                return maxTake > 0 ? query.Take(maxTake) : query;
            }

            var antlrInputStream = new AntlrInputStream(riql);
            var lexer = new RiqlLexer(antlrInputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new RiqlParser(tokenStream);
            var visitor = new RiqlVisitor<T>(query, propertyAccessor, maxTake);
            var result = visitor.Visit(parser.start());
            return result;
        }

        public static Expression<Func<T, bool>> GetRsqlPredicate<T>(string? rsql, ComparisonBuilder comparisonBuilder)
        {
            comparisonBuilder = comparisonBuilder ?? throw new ArgumentNullException(nameof(comparisonBuilder));
            if (string.IsNullOrWhiteSpace(rsql))
            {
                throw new ArgumentNullException(nameof(rsql));
            }

            var antlrInputStream = new AntlrInputStream(rsql);
            var lexer = new RsqlLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new RsqlParser(commonTokenStream);
            var visitor = new RsqlVisitor<T>(comparisonBuilder);
            var predicate = visitor.Visit(parser.start());
            return predicate;
        }
    }
}