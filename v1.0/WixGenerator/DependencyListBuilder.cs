using System.Text.Json;
using System.Text.RegularExpressions;

namespace WixGenerator {

    public sealed class DependencyListBuilder {

        private readonly string _workPath;
        private readonly List<string> _folders = new List<string>();
        private readonly List<string> _dependencyFiles = new List<string>();
        private readonly List<Regex> _exclusions = new List<Regex>();

        public DependencyListBuilder(string workPath) {

            _workPath = workPath;
        }

        /// <summary>
        /// Afegeix una carpeta a la llista de procesament
        /// </summary>
        /// <param name="folder">La carpeta a explorar</param>
        /// <returns>El propi objecte.</returns>
        /// 
        public DependencyListBuilder AddFolder(string folder) {

            if (!_folders.Contains(folder))
                _folders.Add(folder);

            return this;
        }

        /// <summary>
        /// Afegeix un fitxer de dependencies (.deps.json) a la llista de processament
        /// </summary>
        /// <param name="file">El arxiu.</param>
        /// <returns>El propi objecte.</returns>
        /// 
        public DependencyListBuilder AddDependencyFile(string file) {

            if (!_dependencyFiles.Contains(file))
                _dependencyFiles.Add(file);

            return this;
        }

        /// <summary>
        /// Afegeix una patro d'exclussio per filtrar les dependencies.
        /// </summary>
        /// <param name="pattern">El patro com a expressio regulkar.</param>
        /// <returns>El propi objecte.</returns>
        /// 
        public DependencyListBuilder AddExclusionPattern(string pattern) {

            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _exclusions.Add(regex);

            return this;
        }

        /// <summary>
        /// Contrueix la llista de dependencies a partir dels fitxers i carpetes afegides.
        /// </summary>
        /// <returns>Enumeracio de les dependencies.</returns>
        /// 
        public IEnumerable<string> Build() {

            var list = new List<string>();

            foreach (var dependencyFile in _dependencyFiles)
                AppendFromDependencyFile(dependencyFile, list);

            foreach (var folder in _folders)
                AppendFromFolder(folder, list);

            foreach (var regex in _exclusions)
                list.RemoveAll(i => regex.IsMatch(i));

            return list;
        }

        /// <summary>
        /// Afegeix els fitxers .dll trobats dins la carpeta indicada i les seves subcarpetes.
        /// </summary>
        /// <param name="folder">La carpeta a explorar.</param>
        /// 
        public void AppendFromFolder(string folder, IList<string> list) {

            var files = Directory.GetFiles(Path.Combine(_workPath, folder), "*.dll", SearchOption.AllDirectories);
            foreach (var file in files) {
                var f = Path.GetRelativePath(_workPath, file);
                if (!list.Contains(f))
                    list.Add(f);
            }
        }

        /// <summary>
        /// Llegeix i desserialitza un .deps.json i imprimeix la llista de fitxers runtime resolts.
        /// </summary>
        /// <param name="file">El fitxer .deps.json a processar.</param>
        /// 
        public void AppendFromDependencyFile(string file, IList<string> list) {

            var files = ExtractRuntimeFiles(Path.Combine(_workPath, file));
            foreach (var f in files) {
                var fName = Path.GetFileName(f);
                if (!list.Contains(fName))
                    list.Add(fName);
            }
        }

        /// <summary>
        /// Extrau la llista de rutes (resoltes de forma relativa) d'assemblies i natius que apareixen a
        /// l'objecte "runtime"/"native" del .deps.json. Si existeix una entrada a "libraries" amb "path",
        /// es combina: path + runtimeKey. Si no, es retorna la clau tal qual.
        /// Retorna la llista sense duplicats i sense orden concret garantitzat.
        /// </summary>
        /// 
        public IReadOnlyList<string> ExtractRuntimeFiles(string depsFilePath) {

            var result = new List<string>(capacity: 128);

            string json = File.ReadAllText(depsFilePath);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Diccionari que conte la clau de la llibraria i el seu path
            //
            var libraryPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Ompla la llista de llibraries
            if (root.TryGetProperty("libraries", out var libraryElement) && libraryElement.ValueKind == JsonValueKind.Object) {
                foreach (var libraryProperty in libraryElement.EnumerateObject()) {
                    if (libraryProperty.Value.ValueKind == JsonValueKind.Object &&
                        libraryProperty.Value.TryGetProperty("path", out var pathProperty) &&
                        pathProperty.ValueKind == JsonValueKind.String) {

                        libraryPaths[libraryProperty.Name] = pathProperty.GetString() ?? String.Empty;
                    }
                }
            }

            if (root.TryGetProperty("targets", out var targetElement) && targetElement.ValueKind == JsonValueKind.Object) {
                foreach (var targetProperty in targetElement.EnumerateObject()) {

                    if (targetProperty.Value.ValueKind != JsonValueKind.Object)
                        continue;

                    foreach (var packageProperty in targetProperty.Value.EnumerateObject()) {
                        if (packageProperty.Value.ValueKind != JsonValueKind.Object)
                            continue;

                        if (packageProperty.Value.TryGetProperty("runtime", out var runtimeElement) && runtimeElement.ValueKind == JsonValueKind.Object) {
                            foreach (var runtimeFileProp in runtimeElement.EnumerateObject()) {
                                string runtimeKey = runtimeFileProp.Name;
                                string resolved = ResolvePath(libraryPaths, packageProperty.Name, runtimeKey);
                                if (!result.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                                    result.Add(resolved);
                            }
                        }

                        // native (ex. runtimes/win-x64/native/..)
                        if (packageProperty.Value.TryGetProperty("native", out var nativeElement) && nativeElement.ValueKind == JsonValueKind.Object) {
                            foreach (var nativeFileProp in nativeElement.EnumerateObject()) {
                                string nativeKey = nativeFileProp.Name;
                                string resolved = ResolvePath(libraryPaths, packageProperty.Name, nativeKey);
                                if (!result.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                                    result.Add(resolved);
                            }
                        }

                        // resources (opcional)
                        if (packageProperty.Value.TryGetProperty("resources", out var resourcesElem) && resourcesElem.ValueKind == JsonValueKind.Object) {
                            foreach (var resFileProp in resourcesElem.EnumerateObject()) {
                                string resKey = resFileProp.Name;
                                string resolved = ResolvePath(libraryPaths, packageProperty.Name, resKey);
                                if (!result.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                                    result.Add(resolved);
                            }
                        }
                    }
                }
            }

            return result;
        }

        // Resolveix intentant combinar library.path amb la clau del runtime. Si no existeix path
        // retorna la clau original. Normalitza separadors segons la plataforma.
        //
        private static string ResolvePath(Dictionary<string, string> libraryPaths, string packageKey, string relativeKey) {

            // Si tenim library.path per al package, combinem
            if (libraryPaths.TryGetValue(packageKey, out var libPath) && !string.IsNullOrEmpty(libPath)) {

                // normalitzar separadors a la plataforma
                string rel = relativeKey.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                string lib = libPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

                // Si relativeKey ja és només un nom (ex: "MyApp.dll"), Path.Join igual funciona
                try {
                    // No cridem GetFullPath per no forçar existència; retornem la ruta relativa combinada
                    return Path.Join(lib, rel);
                }
                catch {
                    // En cas d'error estrany, fallback a concatenació
                    return lib + Path.DirectorySeparatorChar + rel;
                }
            }

            // No hi ha library.path: retornem la clau tal qual (normalitzada)
            return relativeKey.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}
