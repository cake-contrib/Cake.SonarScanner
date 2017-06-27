using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.SonarScanner
{
    /// <summary>
    /// Contains functionality for using the Sonar Scanner tool
    /// </summary>
    /// <code>
    /// <![CDATA[
    /// #addin "nuget:?package=Cake.SonarScanner"
    /// ]]>
    /// </code>
    [CakeAliasCategory("SonarScanner")]
    public static class SonarScannerAliases
    {
        /// <summary>
        /// Runs SonarScanner with the default settings
        /// </summary>
        /// <example>
        /// <para>Use the #addin preprocessor directive</para>
        /// <code>
        /// <![CDATA[
        /// #addin "nuget:?package=Cake.SonarScanner"
        /// ]]>
        /// </code>
        /// <para>Cake task:</para>
        /// <code>
        /// <![CDATA[
        /// Task("SomeTask").Does(() =>  SonarScanner());
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        [CakeMethodAlias]
        public static void SonarScanner(this ICakeContext context)
        {
            SonarScanner(context, new SonarScannerSettings());
        }

        /// <summary>
        /// Runs SonarScanner with the specified settings
        /// </summary>
        /// <example>
        /// <para>Use the #addin preprocessor directive</para>
        /// <code>
        /// <![CDATA[
        /// #addin "nuget:?package=Cake.SonarScanner"
        /// ]]>
        /// </code>
        /// <para>Cake task:</para>
        /// <code>
        /// <![CDATA[
        /// Task("SomeTask").Does(() =>  SonarScanner(new SonarScannerSettings
        /// {
        /// });
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="settings">The SonarScannerSettings</param>
        [CakeMethodAlias]
        public static void SonarScanner(this ICakeContext context, SonarScannerSettings settings)
        {
            var runner = new SonarScannerTool(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            runner.RunSonarScanner(settings);
        }
    }
}