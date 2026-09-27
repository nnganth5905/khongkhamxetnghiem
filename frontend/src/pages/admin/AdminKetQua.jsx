import React, { useEffect, useState } from 'react';
import api from '../../services/api';

export default function AdminKetQua() {
  const [data, setData] = useState([]);

  useEffect(() => {
    api.get('/admin/results')
      .then(res => setData(res.data?.data || []))
      .catch(err => console.error(err));
  }, []);

  return (
    <div>
      <h1>Quản lý kết quả xét nghiệm</h1>
      <table>
        <thead>
            <tr>    
                <th>Mã bệnh nhân</th>
                <th>Mã mẫu</th>
                <th>Kết quả</th>
                <th>Ngày xét nghiệm</th>
            </tr>
        </thead>
        <tbody>
            {data.map((item, index) => (
                <tr key={index}>
                    <td>{item.patientCode}</td>
                    <td>{item.specimenCode}</td>
                    <td>{item.result}</td>
                    <td>{item.testDate}</td>
                </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
}