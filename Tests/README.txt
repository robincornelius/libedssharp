EDS regression tests
====================

Run ./run-tests.ps1 with Visual Studio MSBuild and the .NET Framework 4.8
targeting pack installed. It restores the owning solution and runs the xUnit
2.9.3 .NET 4.8 console runner. Select -Platform x64 or x86 and -Configuration
Debug or Release for one build, or use -NoBuild after a full solution rebuild.

Visual Studio test discovery uses xunit.runner.visualstudio 4.0.1; the project
also uses xunit.analyzers 2.2.0. The test adapter's declared .NET Framework
support packages are included in packages.config so restore is reproducible.

The suite covers both legacy and CANopenNode v4 PDO communication layouts,
legacy exporter output, and XDD round trips for single-byte and two-byte
subindices, including sparse/high subindices and invalid encodings. The seven
historical failures have been resolved: five stale expectations were corrected
and the legacy XDD importer now decodes the complete subindex value.
