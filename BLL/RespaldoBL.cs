using DAL;

namespace BLL
{
    public class RespaldoBL
    {
        private readonly RespaldoDAL _respaldoDAL = new RespaldoDAL();

        public string RealizarBackup(string backupPath)
        {
            return _respaldoDAL.RealizarBackup(backupPath);
        }
        public void RealizarRestore(string restorePath)
        {
            _respaldoDAL.RealizarRestore(restorePath);
        }
    }
}