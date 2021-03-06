﻿using CodeSourceGenerator.Commmon.Helpers;
using CodeSourceGenerator.Common;
using CodeSourceGenerator.Common.Logging;
using CodeSourceGenerator.Models.Generations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeSourceGenerator.Engines
{
    public class GenerationEngine
    {
        private Logger _logger;

        public GenerationEngine(Logger logger)
        {
            _logger = logger;
        }

        public int CurrentGeneration { get; protected set; } = 0;

        private readonly Dictionary<string, List<GenerationModel<RenderResultModel>>> _generations = new Dictionary<string, List<GenerationModel<RenderResultModel>>>();

        public List<KeyValuePair<string, RenderResultModel>> CurrentGenerations => GetGeneration(CurrentGeneration);
        public List<KeyValuePair<string, RenderResultModel>> PreviousGenerations => GetGeneration(CurrentGeneration - 1);

        public void AddToCurrentGeneration(RenderResultModel renderResult)
        {
            GenerationModel<RenderResultModel> newGeneration = new GenerationModel<RenderResultModel>(CurrentGeneration, renderResult);

            foreach (string fileName in renderResult.OutputPaths)
            {
                List<GenerationModel<RenderResultModel>> previousGens = _generations.ContainsKey(fileName) ? _generations[fileName] : null;

                if (previousGens is null || !previousGens.Any())
                {
                    _generations.Add(fileName, new List<GenerationModel<RenderResultModel>> { newGeneration });
                }
                else
                {
                    GenerationModel<RenderResultModel> currentGen = previousGens.FirstOrDefault(g => g.Generation == CurrentGeneration);
                    if (currentGen == null)
                        previousGens.Add(newGeneration);
                    else
                        currentGen.Model.AppendResultAfter(renderResult.Result);
                }
            }
        }

        public void NewGeneration()
        {
            CurrentGeneration++;
        }


        public void PublishGeneration(SourceGeneratorContext context)
        {
            foreach (KeyValuePair<string, RenderResultModel> pair in PreviousGenerations)
            {
                FileHelper.DeleteFile(pair.Key);
            }

            foreach (KeyValuePair<string, RenderResultModel> pair in CurrentGenerations)
            {
                try
                {
                    SourceText source = SourceText.From(pair.Value?.Result, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(pair.Key))
                    {
                        if (pair.Value.BuilderConfig.Output.AddToCompilation)
                        {
                            context.AddSource(Path.GetFileName(pair.Key), source);
                        }

                        FileHelper.WriteFile(pair.Key, source.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error rendering templates", $"Could not render template output {pair.Key}", ex);
                }
            }
        }

        private List<KeyValuePair<string, RenderResultModel>> GetGeneration(int generation)
            => _generations?.Where(p => p.Value.Any(g => g.Generation == generation))?.Select(g => new KeyValuePair<string, RenderResultModel>(g.Key, g.Value.FirstOrDefault(g => g.Generation == generation).Model))?.ToList();
    }
}
