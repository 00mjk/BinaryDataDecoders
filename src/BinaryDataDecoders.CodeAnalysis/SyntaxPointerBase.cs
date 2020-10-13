﻿using BinaryDataDecoders.ToolKit.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BinaryDataDecoders.CodeAnalysis
{
    internal abstract class SyntaxPointerBase<TItem> : ISyntaxPointer
    {
        protected SyntaxPointerBase(TItem item, ISyntaxPointer? owner)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Owner = owner;

            ChildrenEnumerator = this.Children.GetReversibleEnumerator();
            AttributesEnumerator = this.Attributes.GetReversibleEnumerator();
        }

        public ISyntaxPointer? Owner { get; }
        public TItem Item { get; }

        protected virtual IEnumerable<ISyntaxPointer> GetChildren() { yield break; }
        public IEnumerable<ISyntaxPointer> Children => GetChildren();

        protected virtual IEnumerable<(XName name, object value)> GetAttributes()
        {
           // yield return ("RawType", Item.GetType().AssemblyQualifiedName);
            if (Item is SyntaxNode node)
            {
                yield return ("RawKind", node.RawKind);
                yield return ("Type",nameof(SyntaxNode)[6..]);
            }
            else if (Item is SyntaxToken token)
            {
                yield return ("RawKind", token.RawKind);
                yield return ("Type", nameof(SyntaxToken)[6..]);
            }
            else if (Item is SyntaxNodeOrToken nodeOrToken)
            {
                yield return ("RawKind", nodeOrToken.RawKind);
                yield return ("Type", nameof(SyntaxNodeOrToken)[6..]);
            }
            else if (Item is SyntaxTrivia trivia)
            {
                yield return ("RawKind", trivia.RawKind);
                yield return ("Type", nameof(SyntaxTrivia)[6..]);
            }

            //yield return ("Raw", Item.ToString());
            yield break;
        }
        public IEnumerable<ISyntaxPointer> Attributes
        {
            get
            {
                foreach (var attribute in GetAttributes())
                    yield return attribute.ToSyntaxPointer(this);
            }
        }

        public bool HasChildren => Children.Any();
        public bool HasAttributes => Attributes.Any();

        public string NamespaceUri => $"bdd:CodeAnalysis/{this.GetType().Name[6..^7]}";
        public string Name =>
           $@"{Item switch
           {
               SyntaxNode node when node.Language == "C#" => node.Kind(),
               SyntaxToken token when token.Language == "C#" => token.Kind(),
               SyntaxNodeOrToken nodeOrToken when nodeOrToken.Language == "C#" => nodeOrToken.Kind(),
               SyntaxTrivia trivia when trivia.Language == "C#" => trivia.Kind(),

               _ => Item?.GetType().Name ?? "UNKNOWN"
           }}";
        public virtual string Value => ToString();

        public IReversibleEnumerator<ISyntaxPointer> ChildrenEnumerator { get; }
        public IReversibleEnumerator<ISyntaxPointer> AttributesEnumerator { get; }


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public override string ToString() => Item.ToString();

#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}