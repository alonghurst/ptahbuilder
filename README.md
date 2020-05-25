# Status

[![Build Status](https://goatstruck.visualstudio.com/PtahBuilder/_apis/build/status/alonghurst.ptahbuilder?branchName=master)](https://goatstruck.visualstudio.com/PtahBuilder/_build/latest?definitionId=5&branchName=master)

# Overview

Instantiate a DataGeneratorFactory with a target list of types to generate data instances for and a path to a folder containing data files. The DataGeneratorFactory will process these files into type instances which can be inspected and modified in C# before being written to various other generators.

# Concepts

Data Entity - the things being generated

DataGeneratorFactory - root class instance which manages the generation of all the types. Often some of the generation steps will require access to the other entites and so the DataGeneratorFactory will handle this orchestration.

DataGenerator<T> - each instance of this class is responsible for instantiating entity instances and configuring them from data files on disk. The default file type is Yaml and loaded by the YamlToBaseDataMapper<T> which will instantiate an empty instance of T for each file and then use reflection to populate the properties of the entity with the data from the file. This operation yields a Dictionary<T, MetadataCollection> where T is an instance of the type that was generated and MetadataCollection is a Dictionary string of key value pairs which can be used to contain Metadata about or added during the generation process.

IOperation<T> - after the DataGenerator<T> has loaded the entities from disk it will execute a series of IOperation<T> against each entity. These operations can fully change the entire entity or entity collection.

MetadataResolver<T> - handles metadata pertaining to the entity such as getting or setting the Id and managing the various filepaths for the generated output.

SecondaryGenerator<T> - contains methods marked with the [GenerateAttribute] which act upon data entities doing transformations similar to the IOperations<T>. The difference is that SecondaryGenerators are loaded once all of the DataGenerators have finished loading the initial versions of the entities. That means that the methods marked with [Generate] in SecondaryGenerator classes can take arrays of other entities as parameters. This is particularly useful when entity type A has a reference to entity type B and you wish to validate that instances of entity A can correctly be matched with a loaded entity B.

# Configuration

DataGenerator<T> and MetaDataResolver<T> have working base classes provided but they can be inherited from per type T if desired. The builder will try to find non-default derrived types in any loaded assemblies and prefer to use them over the provided base classes.

To add IOperations<T> derrive from DataGenerator<T> and add to the GetOperations method. You will probably also want to yield the results of the base GetOperations methods too.

Any SecondaryGenerator<T> types will automatically be found (if they are contained in loaded assemblies) and instantiated. Methods marked with [Generate] will be called on these instances.
