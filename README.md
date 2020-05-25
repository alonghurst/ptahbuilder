# Status

[![Build Status](https://goatstruck.visualstudio.com/PtahBuilder/_apis/build/status/alonghurst.ptahbuilder?branchName=master)](https://goatstruck.visualstudio.com/PtahBuilder/_build/latest?definitionId=5&branchName=master)

# Overview

Instantiate a DataGeneratorFactory with a target list of types to generate data instances for and a path to a folder containing data files. The DataGeneratorFactory will process these files into type instances which can be inspected and modified in C# before being written to various other generators.

Most types defined in the Build system can be inherited from and derrived types are instantiated at run-time for each type being loaded.

# Concepts

Data Entity - the things being generated

DataGeneratorFactory - root class instance which manages the generation of all the types. Often some of the generation steps will require access to the other entites and so the DataGeneratorFactory will handle this orchestration.

MetadataResolver - handles metadata pertaining to the entity such as getting or setting the Id and managing the various filepaths for the generated output.

DataLoader - each instance of this class is responsible for instantiating entity instances and configuring them from data files on disk. The default file type is Yaml and loaded by the YamlToBaseDataMapper<T> which will instantiate an empty instance of T for each file and then use reflection to populate the properties of the entity with the data from the file. This operation yields a Dictionary<T, MetadataCollection> where T is an instance of the type that was generated and MetadataCollection is a Dictionary string of key value pairs which can be used to contain Metadata about or added during the generation process.

Operation - after the DataLoader has loaded the entities from disk it will execute a series of Operations against each entity. The DataGenerator factory will first find derrived types that extend BaseOperationProvider<T>, failing that will simply instantiate a BaseOperationProvider<T>. Then any concrete implementations of Operation will be instantiated. All Operation instances for all entity types will be loaded. They will then be executed in order according to the Priority property of each instance. 

Operations are instantiated with an ILogger, MetadataResolver<T> and a Dictionary<T, MetadataCollection> collection of entities to process the operation for. Any Methods marked with the [OperateAttribute] will be considered for execution. The build system will analyze each method and attempt to pass arguments to satisfy the parameters with  array of other entities that have been loaded. This is particularly useful when entity type A has a reference to entity type B and you wish to validate that instances of entity A can correctly be matched with a loaded entity B.

# Configuration & Useful Base Classes

MetaDataResolver<T> have working base classes provided but they can be inherited from per type T if desired. The builder will try to find non-default derrived types in any loaded assemblies and prefer to use them over the provided base classes.

You can inherit from Validator<T> and override the StringPropertiesToValidate method to specify which string properties should be validated as not empty.

