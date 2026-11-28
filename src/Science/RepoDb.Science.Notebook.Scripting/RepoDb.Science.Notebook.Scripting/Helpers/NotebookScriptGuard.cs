using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Checks a compiled script that has no database access before it runs. The check is by symbol, so a renamed type
/// or a split string does not get around it. It is not a sandbox.
/// </summary>
internal sealed class NotebookScriptGuard
{
    #region Private Variables

    private readonly NotebookScriptingMessages _messages;
    private readonly NotebookScriptRestrictions _restrictions;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a guard.
    /// </summary>
    /// <param name="messages">The texts of the problems.</param>
    public NotebookScriptGuard(NotebookScriptingMessages messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        _messages = messages;
        _restrictions = new NotebookScriptRestrictions(messages);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets one text for each use of what is not allowed. The same one is told once.
    /// </summary>
    /// <param name="compilation">The compilation of the script.</param>
    /// <param name="importLines">The number of lines the imports take before the code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The problems.</returns>
    public IReadOnlyList<string> Check(Compilation compilation,
        int importLines,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(compilation);
        return compilation.SyntaxTrees
            .SelectMany(tree => Check(compilation.GetSemanticModel(tree),
                importLines,
                cancellationToken))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the problems of a syntax tree.
    /// </summary>
    /// <param name="model">The semantic model of the tree.</param>
    /// <param name="importLines">The number of lines of the imports.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The problems.</returns>
    private IEnumerable<string> Check(SemanticModel model,
        int importLines,
        CancellationToken cancellationToken)
    {
        foreach (var node in model.SyntaxTree.GetRoot(cancellationToken)
            .DescendantNodes(descendIntoTrivia: true))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ReasonOf(model,
                node,
                cancellationToken) is { } reason)
            {
                yield return _messages.ForbiddenUse(reason,
                    LineOf(node,
                        importLines));
            }
        }
    }

    /// <summary>
    /// Gets the line of the code a node is on, where a use in the imports counts as the first line.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="importLines">The number of lines of the imports.</param>
    /// <returns>The line.</returns>
    private static int LineOf(SyntaxNode node,
        int importLines) => Math.Max(1,
        node.GetLocation()
            .GetLineSpan()
            .StartLinePosition.Line + 1 - importLines);

    /// <summary>
    /// Gets why a node may not be in the script.
    /// </summary>
    /// <param name="model">The semantic model.</param>
    /// <param name="node">The node.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The reason, or null when it may.</returns>
    private string? ReasonOf(SemanticModel model,
        SyntaxNode node,
        CancellationToken cancellationToken)
    {
        if (node is ReferenceDirectiveTriviaSyntax or LoadDirectiveTriviaSyntax)
        {
            return _messages.ForbiddenDirective();
        }
        return node is IdentifierNameSyntax or GenericNameSyntax or BaseObjectCreationExpressionSyntax
            ? _restrictions.ReasonOf(model.GetSymbolInfo(node,
                cancellationToken).Symbol)
            : null;
    }

    #endregion
}
