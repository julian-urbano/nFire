using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nFire.Core;

namespace nFire.Base
{
    /// <summary>
    /// A base task.
    /// </summary>
    public class Task<T> : ITask<T> where T : IResult
    {
        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the document collection.
        /// </summary>
        public nFire.Core.IItemCollection<IDocument> Documents
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the query collection.
        /// </summary>
        public nFire.Core.IItemCollection<IQuery> Queries
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the system collection.
        /// </summary>
        public nFire.Core.IItemCollection<ISystem> Systems
        {
            get;
            protected set;
        }

        /// <summary>
        /// The ground truth runs, indexed by query ID.
        /// </summary>
        /// <remarks>query->run</remarks>
        protected SortedDictionary<string, IRun<T>> GroundTruthsByQuery
        {
            get;
            set;
        }
        /// <summary>
        /// The system runs, indexed by system and query.
        /// </summary>
        /// <remarks>system->query->run</remarks>
        protected SortedDictionary<string, SortedDictionary<string, IRun<T>>> RunsBySystem
        {
            get;
            set;
        }
        /// <summary>
        /// The system runs, indexed by query and system.
        /// </summary>
        /// <remarks>query->system->run</remarks>
        protected SortedDictionary<string, SortedDictionary<string, IRun<T>>> RunsByQuery
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new base task with the specified name and collections of documents, queries and systems.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="dCol">The document collection.</param>
        /// <param name="qCol">The query collection.</param>
        /// <param name="sCol">The system collection.</param>
        public Task(string name, nFire.Core.IItemCollection<IDocument> dCol, nFire.Core.IItemCollection<IQuery> qCol, nFire.Core.IItemCollection<ISystem> sCol)
        {
            this.Name = name;
            this.Documents = dCol;
            this.Queries = qCol;
            this.Systems = sCol;

            this.GroundTruthsByQuery = new SortedDictionary<string, IRun<T>>();
            this.RunsBySystem = new SortedDictionary<string, SortedDictionary<string, IRun<T>>>();
            this.RunsByQuery = new SortedDictionary<string, SortedDictionary<string, IRun<T>>>();
        }
        /// <summary>
        /// Creates a new base task with base collections of documents, queries and systems.
        /// </summary>
        public Task()
            : this("Unnamed", new DocumentCollection(), new QueryCollection(), new SystemCollection())
        {
        }

        /// <summary>
        /// Uses the specified reader to read runs from the specified file and adds them as ground truth .
        /// </summary>
        /// <param name="reader">The reader to read runs from the file.</param>
        /// <param name="path">The path to the file containing the runs.</param>
        public void AddGroundTruths(IRunReader<T> reader, string path)
        {
            IEnumerable<IRun<T>> runs = reader.Read(path, this);
            this.AddGroundTruths(runs);
        }
        /// <summary>
        /// Adds the specified runs as ground truth.
        /// </summary>
        /// <param name="runs">The runs to add as ground truth.</param>
        public void AddGroundTruths(IEnumerable<IRun<T>> runs)
        {
            foreach (IRun<T> run in runs)
                this.AddGroundTruth(run);
        }
        /// <summary>
        /// Adds the specified run as ground truth.
        /// </summary>
        /// <param name="run">The run to add as ground truth.</param>
        public void AddGroundTruth(IRun<T> run)
        {
            this.GroundTruthsByQuery.Add(run.Query.Id, run);
        }

        /// <summary>
        /// Uses the specified reader to read runs from the specified file and adds them as system runs.
        /// </summary>
        /// <param name="reader">The reader to read runs from the file.</param>
        /// <param name="path">The path to the file containing the runs.</param>
        public void AddSystemRuns(IRunReader<T> reader, string path)
        {
            IEnumerable<IRun<T>> runs = reader.Read(path, this);
            this.AddSystemRuns(runs);
        }
        /// <summary>
        /// Adds the specified runs as system runs.
        /// </summary>
        /// <param name="runs">The runs to add as system runs.</param>
        public void AddSystemRuns(IEnumerable<IRun<T>> runs)
        {
            foreach (IRun<T> run in runs)
                this.AddSystemRun(run);
        }
        /// <summary>
        /// Adds the specified run as system run.
        /// </summary>
        /// <param name="run">The run to add as system run.</param>
        public void AddSystemRun(IRun<T> run)
        {
            if (!this.RunsBySystem.ContainsKey(run.System.Id)) {
                this.RunsBySystem.Add(run.System.Id, new SortedDictionary<string, IRun<T>>());
            }
            this.RunsBySystem[run.System.Id][run.Query.Id] = run;

            if (!this.RunsByQuery.ContainsKey(run.Query.Id)) {
                this.RunsByQuery.Add(run.Query.Id, new SortedDictionary<string, IRun<T>>());
            }
            this.RunsByQuery[run.Query.Id][run.System.Id] = run;
        }

        /// <summary>
        /// Evaluates the run of the specified system for the specified query, according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="system">The system to evaluate.</param>
        /// <param name="query">The query to evaluate.</param>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The score returned by the evaluator.</returns>
        public TScore Evaluate<TScore>(ISystem system, IQuery query, IEvaluator<TScore, T> eval)
        {
            IRun<T> gt = this.GroundTruthsByQuery[query.Id];
            IRun<T> run = this.RunsBySystem[system.Id][query.Id];
            return eval.Evaluate(gt, run);
        }
        /// <summary>
        /// Evaluates all runs of the specified query according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="query">The query to evaluate.</param>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        public IDictionary<ISystem, TScore> EvaluateAllSystems<TScore>(IQuery query, IEvaluator<TScore, T> eval)
        {
            Dictionary<ISystem, TScore> scores = new Dictionary<ISystem, TScore>();
            IRun<T> gt = this.GroundTruthsByQuery[query.Id];
            foreach (IRun<T> run in this.RunsByQuery[query.Id].Values) {
                scores.Add(run.System, eval.Evaluate(gt, run));
            }
            return scores;
        }
        /// <summary>
        /// Evaluates all runs of the specified system according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="system">The system to evaluate.</param>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        public IDictionary<IQuery, TScore> EvaluateAllQueries<TScore>(ISystem system, IEvaluator<TScore, T> eval)
        {
            Dictionary<IQuery, TScore> scores = new Dictionary<IQuery, TScore>();
            IEnumerable<IRun<T>> runs = this.RunsBySystem[system.Id].Values;

            foreach (IRun<T> run in runs) {
                IRun<T> gt = this.GroundTruthsByQuery[run.Query.Id];
                scores.Add(run.Query, eval.Evaluate(gt, run));
            }
            return scores;
        }
        /// <summary>
        /// Evaluates all runs according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        public IDictionary<ISystem, IDictionary<IQuery, TScore>> EvaluateAllSystems<TScore>(IEvaluator<TScore, T> eval)
        {
            Dictionary<ISystem, IDictionary<IQuery, TScore>> scores = new Dictionary<ISystem, IDictionary<IQuery, TScore>>();
            foreach (var pair in this.RunsBySystem) {
                ISystem system = this.Systems[pair.Key];
                scores.Add(system, this.EvaluateAllQueries(system, eval));
            }
            return scores;
        }
        /// <summary>
        /// Evaluates all runs according to the specified evaluator.
        /// </summary>
        /// <typeparam name="TScore">The type of score returned by the evaluator.</typeparam>
        /// <param name="eval">The evaluator.</param>
        /// <returns>The scores returned by the evaluator.</returns>
        public IDictionary<IQuery, IDictionary<ISystem, TScore>> EvaluateAllQueries<TScore>(IEvaluator<TScore, T> eval)
        {
            Dictionary<IQuery, IDictionary<ISystem, TScore>> scores = new Dictionary<IQuery, IDictionary<ISystem, TScore>>();
            foreach (var pair in this.RunsByQuery) {
                IQuery query = this.Queries[pair.Key];
                scores.Add(query, this.EvaluateAllSystems(query, eval));
            }
            return scores;
        }
    }
}